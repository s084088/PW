using PwApi.Sockets;

namespace PwApi;


public class DeliveryDB(string ip, int port) : ServerSocket(ip, port)
{
    public void Send(IDeliverySendPackage package)
    {
        BaseSend(package);
    }

    public void AddRecvPackageProcess<T>(Action<T> func) where T : IDeliveryRecvPackage
    {
        BaseAddRecvPackageProcess(func);
    }
}