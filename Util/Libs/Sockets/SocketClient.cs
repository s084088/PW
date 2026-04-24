using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Util.Libs.Sockets;

/// <summary>
/// 裸 TCP 套接字客户端基类。
/// 负责底层连接、断开、收发原始字节流，并通过事件机制对外暴露发送/接收钩子。
/// 派生类需实现 <see cref="Receive(byte[])"/> 以处理收到的原始字节。
/// </summary>
public abstract partial class SocketClient
{
    /// <summary>底层 .NET Socket 实例。</summary>
    private Socket _socket;

    /// <summary>当前是否处于连接运行状态。</summary>
    private bool _isRuning = false;

    /// <summary>是否已建立连接并处于运行中。</summary>
    public bool IsRunning => _isRuning;

    /// <summary>
    /// 连接到指定的 TCP 服务端地址。
    /// 连接建立后会启动后台接收循环。
    /// </summary>
    /// <param name="ip">目标 IP 地址（IPv4/IPv6 字符串）。</param>
    /// <param name="port">目标端口号。</param>
    public void Connect(string ip, int port)
    {
        // 已在运行中则忽略重复连接请求
        if (_isRuning) return;
        _isRuning = true;

        IPEndPoint endPoint = new(IPAddress.Parse(ip), port);
        _socket = new(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _socket.Connect(endPoint);

        // 用一个后台任务驱动阻塞式接收循环，避免阻塞调用线程
        Task.Run(StartRecv);
    }

    /// <summary>
    /// 主动断开当前连接，关闭底层套接字。
    /// 重复调用时不会有副作用。
    /// </summary>
    public void Disconnect()
    {
        if (!_isRuning) return;

        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
        _isRuning = false;
    }

    /// <summary>
    /// 向服务端发送原始字节数组。
    /// 发送前后分别触发 <c>OnSending</c> 与 <c>OnSended</c> 事件，
    /// 若事件处理者将 <see cref="SocketEventArgs.IsHandled"/> 置为 true，则跳过实际发送。
    /// </summary>
    /// <param name="bytes">待发送的原始字节内容。</param>
    protected void Send(byte[] bytes)
    {
        SocketEventArgs args = new(bytes);
        OnSending(args);
        // 若事件钩子已自行处理（IsHandled=true），则不再走真实发送
        if (!args.IsHandled)
        {
            _socket.Send(bytes);
            OnSended(args);
        }
    }

    /// <summary>
    /// 接收回调钩子，由派生类实现具体的字节解析逻辑。
    /// 当一段原始字节从套接字读取完毕且未被事件处理者拦截时被调用。
    /// </summary>
    /// <param name="bytes">本次从套接字读取到的原始字节。</param>
    protected abstract void Receive(byte[] bytes);


    /// <summary>
    /// 后台接收循环：持续从套接字读取数据，并将每段数据派发到独立任务处理。
    /// 当对端关闭连接（读取到 0 字节）或读取过程中发生任何异常时会主动断开。
    /// </summary>
    /// <remarks>
    /// 收到任何异常都会断开连接：循环内部的 catch 不区分异常类型，统一调用 <see cref="Disconnect"/> 退出。
    /// </remarks>
    private void StartRecv()
    {
        // 1KB 接收缓冲区，循环复用
        byte[] container = new byte[1024];
        while (_isRuning)
        {
            try
            {
                int length = _socket.Receive(new ArraySegment<byte>(container), SocketFlags.None);

                // 对端正常关闭连接时 Receive 返回 0
                if (length == 0)
                {
                    Disconnect();
                    break;
                }

                // 拷贝出本次实际读取到的有效字节，避免后续异步处理与下一次接收复用同一缓冲区
                byte[] recBytes = new byte[length];
                Array.Copy(container, 0, recBytes, 0, length);

                // 把解析工作派发到线程池，接收循环立刻回去等下一段数据
                Task.Run(() =>
                {
                    SocketEventArgs args = new(recBytes);
                    OnReveivd(args);
                    // 事件未声明为已处理时，再走派生类实现的 Receive
                    if (!args.IsHandled)
                        Receive(recBytes);
                });
            }
            // 任何异常一律断开连接，不区分类型
            catch
            {
                Disconnect();
                break;
            }
        }
    }
}
