namespace PwApi;


public class DeliveryDB : ServerSocket
{

    public DeliveryDB(string ip, int port) : base(ip, port)
    {

    }

    public void Send(ISendPakcage package)
    {
        BaseSend(package);
    }


    public void AddRecvPackageProcess<T>(Action<T> func) where T : IRecvPackage
    {
        BaseAddRecvPackageProcess(func);
    }
}