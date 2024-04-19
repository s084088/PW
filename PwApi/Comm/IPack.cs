namespace PwApi.Comm;

public interface IPackage
{
    uint Type { get; }
}

public interface ISendPakcage : IPackage
{
    Packets Pack();
}

public interface IRecvPackage : IPackage
{
    void UnPack(UnPackets unPackets);
}

public interface IPackageItem
{
    void PackTo(Packets packets);
}