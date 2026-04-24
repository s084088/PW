namespace PW.Protocol.Comm;

/// <summary>
/// <see cref="BaseClient"/> 的事件分部：集中放置发送前 / 接收后的事件订阅与触发逻辑。
/// </summary>
public partial class BaseClient
{
    /// <summary>
    /// 发送前事件订阅者列表；按注册顺序依次回调，任一回调把
    /// <see cref="ClientSendEventArgs.IsHandled"/> 置为 <c>true</c> 即可中止后续回调。
    /// </summary>
    private readonly List<Action<ClientSendEventArgs>> _sendings = [];

    /// <summary>
    /// 接收后事件订阅者列表；行为同 <see cref="_sendings"/>。
    /// </summary>
    private readonly List<Action<ClientRecvEventArgs>> _recveds = [];

    /// <summary>
    /// 注册一个发送前事件回调。回调会在每次 <see cref="Send(ISendPackage)"/> 真正写套接字前触发，
    /// 可用来记录日志、修改包内容或直接拦截发送。
    /// </summary>
    /// <param name="sending">回调委托。</param>
    public void AddSendingPacketsEvent(Action<ClientSendEventArgs> sending) => _sendings.Add(sending);

    /// <summary>
    /// 触发发送前事件。按注册顺序依次调用所有订阅者；
    /// 一旦某个订阅者把 <see cref="ClientSendEventArgs.IsHandled"/> 置为 <c>true</c>，
    /// 后续订阅者不再被调用，外层也会跳过实际的 TCP 发送。
    /// </summary>
    /// <param name="args">事件参数，包含底层 <see cref="SendPackets"/> 与业务 <see cref="ISendPackage"/>。</param>
    protected virtual void OnSendingPackets(ClientSendEventArgs args)
    {
        foreach (Action<ClientSendEventArgs> s in _sendings)
        {
            if (args.IsHandled) return;
            s.Invoke(args);
        }
    }


    /// <summary>
    /// 注册一个收包后事件回调。回调会在每次成功解出主动推送包后触发，
    /// 可用来记录日志、读取包内容或拦截后续默认派发。
    /// </summary>
    /// <param name="sending">回调委托。</param>
    public void AddRecvedPacketsEvent(Action<ClientRecvEventArgs> sending) => _recveds.Add(sending);

    /// <summary>
    /// 触发收包后事件。按注册顺序依次调用所有订阅者；
    /// 一旦某个订阅者把 <see cref="ClientRecvEventArgs.IsHandled"/> 置为 <c>true</c>，
    /// 后续订阅者不再被调用，并且 <see cref="Analysis"/> 会跳过默认的
    /// <see cref="ProcessPackge"/> 派发。
    /// </summary>
    /// <param name="args">事件参数，包含底层 <see cref="RecvPackets"/> 与业务 <see cref="IRecvPackage"/>。</param>
    protected virtual void OnRecvedPackets(ClientRecvEventArgs args)
    {
        foreach (Action<ClientRecvEventArgs> s in _recveds)
        {
            if (args.IsHandled) return;
            s.Invoke(args);
        }
    }



}

/// <summary>
/// 发送前事件参数：携带底层发送容器与业务包对象，
/// 订阅者把 <see cref="IsHandled"/> 置为 <c>true</c> 即可拦截实际的套接字写入。
/// </summary>
/// <param name="sendPackets">底层发送容器。</param>
/// <param name="sendPackage">业务发送包。</param>
public class ClientSendEventArgs(SendPackets sendPackets, ISendPackage sendPackage)
{
    /// <summary>底层协议发送容器（已打包好待写出的字节）。</summary>
    public SendPackets SendPackets { get; set; } = sendPackets;

    /// <summary>业务层发送包对象。</summary>
    public ISendPackage SendPackage { get; set; } = sendPackage;

    /// <summary>
    /// 是否已被订阅者处理。置为 <c>true</c> 后，后续订阅者不再被调用，
    /// 且外层 <see cref="BaseClient.Send(ISendPackage)"/> 会跳过真正的 TCP 发送。
    /// </summary>
    public bool IsHandled { get; set; }
}

/// <summary>
/// 收包后事件参数：携带底层接收容器与已反序列化的业务包对象，
/// 订阅者把 <see cref="IsHandled"/> 置为 <c>true</c> 即可拦截后续默认派发。
/// </summary>
/// <param name="recvPackets">底层接收容器。</param>
/// <param name="recvPackage">业务接收包；当未在 <c>RecvPackageRegister</c> 注册时可能为 <c>null</c>。</param>
public class ClientRecvEventArgs(RecvPackets recvPackets, IRecvPackage recvPackage)
{
    /// <summary>底层协议接收容器（含原始字节与已读游标）。</summary>
    public RecvPackets RecvPackets { get; set; } = recvPackets;

    /// <summary>
    /// 业务层接收包对象。若收到的包类型未在 <c>RecvPackageRegister</c> 注册，
    /// 此属性可能为 <c>null</c>。
    /// </summary>
    public IRecvPackage RecvPackage { get; set; } = recvPackage;

    /// <summary>
    /// 是否已被订阅者处理。置为 <c>true</c> 后，后续订阅者不再被调用，
    /// 且 <see cref="BaseClient.Analysis"/> 会跳过默认的 <see cref="BaseClient.ProcessPackge"/> 派发。
    /// </summary>
    public bool IsHandled { get; set; }
}
