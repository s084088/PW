using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using Util.Libs.Sockets;

namespace PW.Protocol.Comm;

/// <summary>
/// PW 协议客户端核心路由层。
/// 继承自底层裸 TCP 套接字封装 <see cref="SocketClient"/>，
/// 在其之上实现包级别的收发、按 <c>Type</c> 路由派发、以及同步请求-响应等机制。
/// <para>
/// 收发模式共三种：
/// </para>
/// <list type="bullet">
///   <item><description><see cref="Send(ISendPackage)"/> —— 单向发出，不等回应。</description></item>
///   <item>
///     <description>
///       <see cref="Send{TSend, TRecv}(ICallPackage{TSend, TRecv})"/> —— 同步请求-响应；
///       通过 <see cref="_calls"/> 字典按包 <c>Type</c> 占位，最多 <c>await Task.Delay(1)</c> 轮询 1000 ms；
///       超时即静默返回 <c>null</c>，不抛异常。
///     </description>
///   </item>
///   <item>
///     <description>
///       <see cref="AddReceive{T}(Action{T})"/> —— 事件订阅式；
///       收到任意主动推送包后，<see cref="Analysis"/> 通过
///       <see cref="RecvPackageRegister.GetPackage"/> 反射得到包实例，
///       再按类型从 <see cref="_recvActionList"/> 查到回调触发。
///     </description>
///   </item>
/// </list>
/// </summary>
public partial class BaseClient : SocketClient
{
    /// <summary>
    /// 主动推送包的回调映射表：包类型（<see cref="System.Type"/>）映射到一个或多个回调委托。
    /// 由 <see cref="AddReceive{T}"/> 注册，由 <see cref="ProcessPackge"/> 触发。
    /// </summary>
    private readonly Dictionary<Type, Action<IRecvPackage>> _recvActionList = [];

    /// <summary>
    /// 同步请求-响应的占位字典：键为请求包的 <c>Type</c>，值在收到回应前为 <c>null</c>。
    /// <para>
    /// 工作流程：<see cref="Send{TSend, TRecv}"/> 在发送前以请求 <c>Type</c> 占位 <c>null</c>，
    /// 然后轮询等待 <see cref="Analysis"/> 把对应回应包写入此字典。
    /// </para>
    /// 使用 <see cref="ConcurrentDictionary{TKey, TValue}"/> 保证收发线程并发访问安全。
    /// </summary>
    private readonly ConcurrentDictionary<uint, RecvPackets> _calls = [];

    /// <summary>
    /// 单向发送：把一个 <see cref="ISendPackage"/> 打包后通过底层 TCP 套接字发出，不等待回应。
    /// <para>
    /// 发送前会触发 <see cref="OnSendingPackets"/> 事件；
    /// 若任一订阅者把 <see cref="ClientSendEventArgs.IsHandled"/> 置为 <c>true</c>，则取消实际发送。
    /// </para>
    /// </summary>
    /// <param name="package">待发送的业务包对象。</param>
    public void Send(ISendPackage package)
    {
        SendPackets sendPackets = new(package.Type);
        sendPackets.Pack(package);
        byte[] data = sendPackets.GetBytes();

        // 发送前事件，允许外部订阅者修改包或拦截发送
        ClientSendEventArgs args = new(sendPackets, package);
        OnSendingPackets(args);
        if (args.IsHandled) return;

        Send(data);
    }

    /// <summary>
    /// 同步请求-响应：发送一个 <see cref="ICallPackage{TSend, TRecv}"/>，并在 1 秒内等待对端回包。
    /// 收到回应后会就地反序列化到 <c>package.Recv</c>。
    /// </summary>
    /// <typeparam name="TSend">请求体类型，必须实现 <see cref="IPackTo"/>。</typeparam>
    /// <typeparam name="TRecv">回应体类型，必须实现 <see cref="IUnPackFrom"/> 且可无参构造。</typeparam>
    /// <param name="package">请求-响应包对象；调用结束后 <c>Recv</c> 字段会被填充（若有回应）。</param>
    /// <returns>异步等待句柄，<c>await</c> 返回后即可读取 <c>package.Recv</c>。</returns>
    /// <remarks>
    /// 同步等待。最多 1000 ms 内未收到回应即静默返回 null，不抛异常；
    /// 新业务若服务器回包慢于 1 秒会"看起来没回应"。
    /// 如需更长超时或异步回调风格，请考虑改写此方法或改用 <see cref="AddReceive{T}"/> 事件路径。
    /// </remarks>
    public async Task Send<TSend, TRecv>(ICallPackage<TSend, TRecv> package)
        where TSend : IPackTo
        where TRecv : IUnPackFrom, new()
    {
        uint type = package.Type;

        // 在并发字典里以请求 Type 占位 null；Analysis() 收到同类型包时会写入回包
        _calls[type] = null;

        //打包发送
        SendPackets sendPackets = new(type);
        sendPackets.Pack(-1);
        sendPackets.Pack(package.Send);

        byte[] data = sendPackets.GetBytes();
        Send(data);

        //等待回应
        Stopwatch sw = Stopwatch.StartNew();
        // 轮询直到收到回包或超过 1000 ms；await Task.Delay(1) 把控制权让出避免忙等
        while (_calls[type] == null && sw.ElapsedMilliseconds < 1000)
            await Task.Delay(1);
        sw.Stop();
        // 无论成功或超时，都把占位移除以免泄漏
        _calls.TryRemove(type, out RecvPackets recv);

        //处理回应
        // 超时未收到回包：静默返回，不抛异常
        if (recv == null) return;

        // 跳过协议头中的预留 int，再把剩余字节按 TRecv 反序列化
        _ = recv.UnPackInt();
        package.Recv = new();
        package.Recv.UnPackFrom(recv);


    }


    /// <summary>
    /// 注册主动推送包 <typeparamref name="T"/> 的回调；
    /// 若同类型已注册过则把新回调链式追加，否则新建条目。
    /// </summary>
    /// <typeparam name="T">推送包类型，必须实现 <see cref="IRecvPackage"/>。</typeparam>
    /// <param name="func">收到此类包时执行的回调，参数为反序列化后的强类型实例。</param>
    public void AddReceive<T>(Action<T> func) where T : IRecvPackage
    {
        Type packageType = typeof(T);

        if (_recvActionList.ContainsKey(packageType))
        {
            // 已存在则用 += 把新回调追加到委托链上，多个回调按注册顺序触发
            _recvActionList[packageType] += new Action<IRecvPackage>(pkg => func((T)pkg));
        }
        else
        {
            _recvActionList[packageType] = new Action<IRecvPackage>(pkg => func((T)pkg));
        }
    }




    /// <summary>
    /// 拆包
    /// </summary>
    /// <param name="bytes">底层 TCP 套接字读到的原始字节流（可能包含多个粘包）。</param>
    protected override void Receive(byte[] bytes)
    {
        // 一次性把字节流切成多个完整的协议包，再逐个分发
        List<RecvPackets> recvPackets = RecvPackets.GetPackets(bytes);
        foreach (RecvPackets p in recvPackets)
        {
            Analysis(p);
        }
    }

    /// <summary>
    /// 解包并找到对应包
    /// </summary>
    /// <param name="p">已拆出的单个协议包。</param>
    /// <remarks>
    /// 方法内 catch 块为空，所有异常被静默吞掉。
    /// 如需调试收包问题，需在 catch 内临时打日志。
    /// </remarks>
    private void Analysis(RecvPackets p)
    {
        try
        {
            if (_calls.ContainsKey(p.Type))     //Call包
            {
                // 同步请求-响应：把回包写入占位，唤醒 Send&lt;TSend,TRecv&gt; 的轮询循环
                _calls[p.Type] = p;
            }
            else                            //普通包
            {
                // 通过类型注册表反射得到对应的强类型包实例
                IRecvPackage rp = RecvPackageRegister.GetPackage(p.Type);

                if (rp != null)
                {
                    // 把原始字节挂上去，并按业务规则反序列化
                    rp.Data = p.Data;
                    rp.UnPackFrom(p);
                }

                ClientRecvEventArgs args = new(p, rp);

                OnRecvedPackets(args);

                // 没有匹配的注册类型 或 事件已被外部处理，跳过默认派发
                if (rp == null || args.IsHandled) return;

                ProcessPackge(rp);

            }
        }
        // 注意：异常被静默吞掉，调试收包问题时需在此处临时加日志
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// 执行包的处理逻辑
    /// </summary>
    /// <param name="rp">已反序列化好的强类型推送包。</param>
    /// <remarks>
    /// 方法内 catch 块为空，所有异常被静默吞掉。
    /// 如需调试收包问题，需在 catch 内临时打日志。
    /// </remarks>
    private void ProcessPackge(IRecvPackage rp)
    {
        try
        {
            // 按运行时类型查找已注册的回调委托链并依次触发
            if (_recvActionList.TryGetValue(rp.GetType(), out Action<IRecvPackage> action))
            {
                action?.Invoke(rp);
            }
        }
        // 注意：异常被静默吞掉，调试收包问题时需在此处临时加日志
        catch (Exception e)
        {

        }
    }
}
