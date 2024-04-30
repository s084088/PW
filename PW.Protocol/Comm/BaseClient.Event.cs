namespace PW.Protocol.Comm;

public partial class BaseClient
{
    private readonly List<Action<ClientSendEventArgs>> _sendings = [];
    private readonly List<Action<ClientRecvEventArgs>> _recveds = [];

    public void AddSendingPacketsEvent(Action<ClientSendEventArgs> sending) => _sendings.Add(sending);
    protected virtual void OnSendingPackets(ClientSendEventArgs args)
    {
        foreach (Action<ClientSendEventArgs> s in _sendings)
        {
            if (args.IsHandled) return;
            s.Invoke(args);
        }
    }


    public void AddRecvedPacketsEvent(Action<ClientRecvEventArgs> sending) => _recveds.Add(sending);
    protected virtual void OnRecvedPackets(ClientRecvEventArgs args)
    {
        foreach (Action<ClientRecvEventArgs> s in _recveds)
        {
            if (args.IsHandled) return;
            s.Invoke(args);
        }
    }



}

public class ClientSendEventArgs(SendPackets sendPackets, ISendPackage sendPackage)
{
    public SendPackets SendPackets { get; set; } = sendPackets;

    public ISendPackage SendPackage { get; set; } = sendPackage;

    public bool IsHandled { get; set; }
}

public class ClientRecvEventArgs(RecvPackets recvPackets, IRecvPackage recvPackage)
{
    public RecvPackets RecvPackets { get; set; } = recvPackets;

    public IRecvPackage RecvPackage { get; set; } = recvPackage;

    public bool IsHandled { get; set; }
}