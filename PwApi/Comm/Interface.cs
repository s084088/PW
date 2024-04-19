namespace PwApi.Comm;

public interface IPackage
{
    uint Type { get; }
}

public interface ISendPakcage : IPackage
{
    Packets Pack();
}

public interface IDeliverySendPackage: ISendPakcage
{

}

public interface IRecvPackage : IPackage
{
    void UnPack(UnPackets unPackets);
}



public interface IDeliveryRecvPackage : IRecvPackage
{

}

public interface IPackageItem
{
    void PackTo(Packets packets);
}