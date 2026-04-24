using System;
using System.Collections.Generic;

namespace Util.Libs.Sockets;

/// <summary>
/// <see cref="SocketClient"/> 的事件分部：维护发送前、发送后、接收三类回调列表，
/// 提供注册接口以及触发分发逻辑。任一回调将 <see cref="SocketEventArgs.IsHandled"/> 置 true 即终止后续回调链。
/// </summary>
public partial class SocketClient
{
    /// <summary>发送前的回调列表（在真正写入套接字之前触发）。</summary>
    private readonly List<Action<SocketEventArgs>> _sendings = [];

    /// <summary>发送后的回调列表（在成功写入套接字之后触发）。</summary>
    private readonly List<Action<SocketEventArgs>> _sendeds = [];

    /// <summary>接收回调列表（在派生类 Receive 之前触发，可拦截）。</summary>
    private readonly List<Action<SocketEventArgs>> _receives = [];

    /// <summary>注册一个发送前回调。</summary>
    /// <param name="sending">回调委托，可通过 <see cref="SocketEventArgs.IsHandled"/> 阻止后续处理。</param>
    public void AddSendingEvent(Action<SocketEventArgs> sending) => _sendings.Add(sending);

    /// <summary>
    /// 触发发送前事件，按注册顺序逐个调用回调。
    /// 一旦事件参数被标记为已处理，立即停止派发并跳过实际发送。
    /// </summary>
    /// <param name="args">本次发送相关的事件参数。</param>
    protected virtual void OnSending(SocketEventArgs args)
    {
        foreach (Action<SocketEventArgs> s in _sendings)
        {
            // 已被前一个回调处理时立即结束分发
            if (args.IsHandled) return;
            s.Invoke(args);
        }
    }

    /// <summary>注册一个发送后回调。</summary>
    /// <param name="sended">回调委托，在数据已写入套接字后被调用。</param>
    public void AddSendedEvent(Action<SocketEventArgs> sended) => _sendeds.Add(sended);

    /// <summary>
    /// 触发发送后事件，按注册顺序逐个调用回调。
    /// 同样支持中途由回调标记为已处理来截断后续派发。
    /// </summary>
    /// <param name="args">本次发送相关的事件参数。</param>
    protected virtual void OnSended(SocketEventArgs args)
    {
        foreach (Action<SocketEventArgs> s in _sendeds)
        {
            if (args.IsHandled) return;
            s.Invoke(args);
        }
    }

    /// <summary>注册一个接收回调。</summary>
    /// <param name="received">回调委托，在派生类 Receive 之前触发，可通过 IsHandled 阻止其执行。</param>
    public void AddReceivdEvent(Action<SocketEventArgs> received) => _receives.Add(received);

    /// <summary>
    /// 触发接收事件，按注册顺序逐个调用回调。
    /// 任一回调将事件标记为已处理后，循环立即结束。
    /// </summary>
    /// <param name="args">本次接收到的字节数据相关的事件参数。</param>
    protected virtual void OnReveivd(SocketEventArgs args)
    {
        foreach (Action<SocketEventArgs> s in _receives)
        {
            if (args.IsHandled) return;
            s.Invoke(args);
        }
    }
}


/// <summary>
/// 套接字事件参数：携带本次收发的原始字节，并提供 <see cref="IsHandled"/> 标记
/// 供回调链中的处理者声明"已自行处理"，从而阻止后续默认逻辑。
/// </summary>
/// <param name="bytes">事件携带的原始字节内容。</param>
public class SocketEventArgs(byte[] bytes)
{
    /// <summary>事件携带的原始字节数据，可被回调读取或替换。</summary>
    public byte[] Bytes { get; set; } = bytes;

    /// <summary>是否已被回调处理；置 true 后将阻止后续事件派发与默认收发逻辑。</summary>
    public bool IsHandled { get; set; }
}
