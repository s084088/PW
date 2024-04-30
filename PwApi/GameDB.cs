using PwApi.Sockets;

namespace PwApi;

public class GameDB(string ip, int port) : ServerSocket(ip, port)
{
    public void Send(IGameDbSendPackage package)
    {
        BaseSend(package);
    }

    public async Task Send<TSend, TRecv>(IGameDbCallPackage<TSend, TRecv> package) where TSend : ISend where TRecv : IRecv, new()
    {
        await BaseSend(package);
    }

    public void AddRecvPackageProcess<T>(Action<T> func) where T : IGameDbRecvPackage
    {
        BaseAddRecvPackageProcess(func);
    }
}