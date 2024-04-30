using System;
using System.Collections.Generic;

namespace Util.Libs.Sockets;
public partial class SocketClient
{
    private readonly List<Action<SocketEventArgs>> _sendings = [];

    private readonly List<Action<SocketEventArgs>> _sendeds = [];

    private readonly List<Action<SocketEventArgs>> _receives = [];

    public void AddSendingEvent(Action<SocketEventArgs> sending) => _sendings.Add(sending);
    protected virtual void OnSending(SocketEventArgs args)
    {
        foreach (Action<SocketEventArgs> s in _sendings)
        {
            if (args.IsHandled) return;
            s.Invoke(args);
        }
    }

    public void AddSendedEvent(Action<SocketEventArgs> sended) => _sendeds.Add(sended);
    protected virtual void OnSended(SocketEventArgs args)
    {
        foreach (Action<SocketEventArgs> s in _sendeds)
        {
            if (args.IsHandled) return;
            s.Invoke(args);
        }
    }

    public void AddReceivdEvent(Action<SocketEventArgs> received) => _receives.Add(received);
    protected virtual void OnReveivd(SocketEventArgs args)
    {
        foreach (Action<SocketEventArgs> s in _receives)
        {
            if (args.IsHandled) return;
            s.Invoke(args);
        }
    }
}


public class SocketEventArgs(byte[] bytes)
{
    public byte[] Bytes { get; set; } = bytes;
    public bool IsHandled { get; set; }
}