namespace PwApi;

public class GameDB(string ip, int port) : ServerSocket(ip, port)
{
    public void Send(IGameDbSendPackage package)
    {
        BaseSend(package);
    }

    public void Send<TSend, TRecv>(IGameDbCallPackage<TSend, TRecv> package) where TSend : ISend where TRecv : IRecv, new()
    {
        BaseSend(package);
    }

    public void AddRecvPackageProcess<T>(Action<T> func) where T : IGameDbRecvPackage
    {
        BaseAddRecvPackageProcess(func);
    }
}