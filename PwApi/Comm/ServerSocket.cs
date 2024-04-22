using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace PwApi.Comm;
public abstract class ServerSocket
{
    private readonly Socket _socket;
    private readonly Dictionary<Type, Action<IRecvPackage>> _recvPackageProcessDict = [];
    private readonly ConcurrentDictionary<uint, UnPackets> _calls = [];

    public bool IsConnected { get; private set; } = false;


    public ServerSocket(string ip, int port)
    {
        IPEndPoint endPoint = new(IPAddress.Parse(ip), port);
        _socket = new(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _socket.Connect(endPoint);
        IsConnected = true;
        Task.Run(StartRecv);
    }

    protected void BaseSend(ISendPakcage package)
    {
        Logger.LogPackets($"send--{package.GetType().Name}--{package}");
        Packets packets = new();
        package.Pack(packets);
        SendInner(packets, package.Type);

    }

    protected async Task BaseSend<TSend, TRecv>(ICallPakcage<TSend, TRecv> package)
        where TSend : ISend
        where TRecv : IRecv, new()
    {
        Logger.LogPackets($"sendCall--{package.Send}");

        uint type = package.Type;
        _calls[type] = null;

        Packets packets = new();
        packets.Pack(-1);
        package.Send.Pack(packets);
        SendInner(packets, type);

        while (_calls[type] == null)
        {
            await Task.Delay(1);
        }

        _calls.TryRemove(type, out UnPackets p);
        _ = p.UnPackInt();
        package.Recv = new();
        package.Recv.UnPack(p);

        Logger.LogPackets($"recvCall--{package.Recv}");
    }

    protected void BaseAddRecvPackageProcess<T>(Action<T> func) where T : IRecvPackage
    {
        Type packageType = typeof(T);
        if (_recvPackageProcessDict.ContainsKey(packageType))
        {
            _recvPackageProcessDict[packageType] += new Action<IRecvPackage>(pkg => func((T)pkg));
        }
        else
        {
            _recvPackageProcessDict[packageType] = new Action<IRecvPackage>(pkg => func((T)pkg));
        }
    }

    private void SendInner(Packets packets, uint type)
    {
        byte[] data = packets.GetBytes(type);
        _socket.Send(data);

        Logger.LogHex($"sendHex--{data.ToHexString()}");
    }

    private void StartRecv()
    {
        byte[] container = new byte[1024];
        while (true)
        {
            int length = _socket.Receive(new ArraySegment<byte>(container), SocketFlags.None);
            if (length > 0)
            {
                byte[] recBytes = new byte[length];
                Array.Copy(container, 0, recBytes, 0, length);

                Task.Run(() => UnPackUnPackets(recBytes));

                Logger.LogHex($"recvHex--{recBytes.ToHexString()}");
            }
            else
            {
                Logger.LogInfo("连接断开");

                _socket.Close();
                IsConnected = false;
                break;
            }
        }
    }

    private void UnPackUnPackets(byte[] recBytes)
    {
        List<UnPackets> unPackets = UnPackets.GetPackets(recBytes);
        foreach (var p in unPackets)
        {
            Analysis(p);
        }
    }

    private void Analysis(UnPackets p)
    {
        try
        {
            //Call包
            if (_calls.ContainsKey(p.Type))
            {
                _calls[p.Type] = p;
            }
            else//普通包
            {
                IRecvPackage rp = DeliveryRecvManage.GetPackage(p.Type);

                if (rp != null)
                {
                    rp.UnPack(p);

                    ProcessPackge(rp);

                    Logger.LogPackets($"recv--{rp.GetType().Name}--{rp}");
                }
                else
                {
                    Logger.LogInfo($"未找到此包的解析程序--{p}");
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError($"对象解包失败--{p}{Environment.NewLine}{ex}");
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
        }
        catch (Exception e)
        {
            Logger.LogError($"用户处理数据包失败--{rp.GetType().Name}--{e.Message}");
        }
    }
}