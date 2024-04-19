using System.Net;
using System.Net.Sockets;

namespace PwApi.Comm;
public abstract class ServerSocket
{

    private readonly Socket socket;
    private readonly Dictionary<Type, Action<IRecvPackage>> _recvPackageProcessDict = [];

    public bool IsConnected { get; private set; } = false;


    public ServerSocket(string ip, int port)
    {
        IPEndPoint endPoint = new(IPAddress.Parse(ip), port);

        socket = new(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        socket.Connect(endPoint);

        IsConnected = true;

        Task.Run(StartRecv);
    }

    protected void BaseSend(ISendPakcage package)
    {
        Logger.Log($"send--{package.GetType().Name}--{package}");

        Packets packets = package.Pack();

        byte[] data = packets.GetBytes(package.Type);
        string str = data.ToHexString();
        socket.Send(data);

        //Logger.Log("sendHex--" + data.ToHexString());
    }


    protected void BaseAddRecvPackageProcess<T>(Action<T> func) where T : IRecvPackage
    {
        Type packageType = typeof(T);
        if (!_recvPackageProcessDict.ContainsKey(packageType))
        {
            _recvPackageProcessDict[packageType] = new Action<IRecvPackage>(pkg => func((T)pkg));
        }
        else
        {
            _recvPackageProcessDict[packageType] += new Action<IRecvPackage>(pkg => func((T)pkg));
        }
    }

    private void StartRecv()
    {
        while (true)
        {
            byte[] container = new byte[1024];
            int length = socket.Receive(container);
            if (length > 0)
            {
                byte[] recBytes = new byte[length];
                Array.Copy(container, 0, recBytes, 0, length);

                Analysis(recBytes);
            }
            else
            {
                Logger.Log("连接断开");

                socket.Close();
                IsConnected = false;
                break;
            }
        }
    }

    private void Analysis(byte[] container)
    {
        try
        {
            UnPackets p = new(container);

            IRecvPackage rp = DeliveryRecvManage.GetPackage(p.Type);

            if (rp != null)
            {
                rp.UnPack(p);

                Task.Run(() => ProcessPackge(rp));
            }
            else
            {
                Logger.Log($"recvUnPackets--{p}");
            }

            if (p.Unknown.Length > 0)
            {
                Analysis(p.Unknown);
            }
        }
        catch
        {
            Logger.Log("recvHex--" + container.ToHexString());
        }
    }

    private void ProcessPackge(IRecvPackage rp)
    {
        try
        {
            if (_recvPackageProcessDict.TryGetValue(rp.GetType(), out Action<IRecvPackage> action))
            {
                action?.Invoke(rp);
            }

            Logger.Log($"recv--{rp.GetType().Name}--{rp}");
        }
        catch (Exception e)
        {
            Logger.Log($"Process Package Error--{rp.GetType().Name}--{e.Message}");
        }
    }
}