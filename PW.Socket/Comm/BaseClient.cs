using System.Collections.Concurrent;
using System.Diagnostics;
using PW.Protocol.Interfaces;
using PW.Protocol.Packages;
using Util.Libs.Sockets;

namespace PW.Sockets.Comm;
public class BaseClient : SocketClient
{
    private readonly Dictionary<Type, Action<IRecvPackage>> _recvActionList = [];
    private readonly ConcurrentDictionary<uint, RecvPackets> _calls = [];

    public void Send(ISendPackage package)
    {
        SendPackets sendPackets = new(package.Type);
        sendPackets.Pack(package);

        byte[] data = sendPackets.GetBytes();
        Send(data);
    }

    public async Task Send<TSend, TRecv>(ICallPackage<TSend, TRecv> package)
        where TSend : IPackTo
        where TRecv : IUnPackFrom, new()
    {
        uint type = package.Type;

        _calls[type] = null;

        //打包发送
        SendPackets sendPackets = new(type);
        sendPackets.Pack(-1);
        sendPackets.Pack(package.Send);

        byte[] data = sendPackets.GetBytes();
        Send(data);

        //等待回应
        Stopwatch sw = Stopwatch.StartNew();
        while (_calls[type] == null && sw.ElapsedMilliseconds < 1000) await Task.Delay(1);
        sw.Stop();
        _calls.TryRemove(type, out RecvPackets recv);

        //处理回应
        if (recv == null) return;

        _ = recv.UnPackInt();
        package.Recv = new();
        package.Recv.UnPackFrom(recv);


    }


    public void AddReceive<T>(Action<T> func) where T : IRecvPackage
    {
        Type packageType = typeof(T);

        if (_recvActionList.ContainsKey(packageType))
        {
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
    /// <param name="bytes"></param>
    protected override void Receive(byte[] bytes)
    {
        List<RecvPackets> recvPackets = RecvPackets.GetPackets(bytes);
        foreach (RecvPackets p in recvPackets)
        {
            Analysis(p);
        }
    }

    /// <summary>
    /// 解包并找到对应包
    /// </summary>
    /// <param name="p"></param>
    private void Analysis(RecvPackets p)
    {
        try
        {
            if (_calls.ContainsKey(p.Type))     //Call包
            {
                _calls[p.Type] = p;
            }
            else                            //普通包
            {
                IRecvPackage rp = RecvPackageRegister.GetPackage(p.Type);

                if (rp != null)
                {
                    rp.UnPackFrom(p);

                    ProcessPackge(rp);
                }
                else
                {

                }
            }
        }
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// 执行包的处理逻辑
    /// </summary>
    /// <param name="rp"></param>
    private void ProcessPackge(IRecvPackage rp)
    {
        try
        {
            if (_recvActionList.TryGetValue(rp.GetType(), out Action<IRecvPackage> action))
            {
                action?.Invoke(rp);
            }
        }
        catch (Exception e)
        {

        }
    }
}