namespace PwApi.Comm;

public interface IPackage
{
    uint Type { get; }
}

public interface ISend
{
    void Pack(Packets p);
}

public interface IRecv
{
    void UnPack(UnPackets p);
}



public interface ISendPakcage : IPackage, ISend
{

}

public interface IRecvPackage : IPackage, IRecv
{
}

public interface ICallPakcage<TSend, TRecv>: IPackage where TSend : ISend where TRecv : IRecv,new()
{
    TSend Send { get; set; }

    TRecv Recv { get; set; }
}



public interface IDeliverySendPackage : ISendPakcage
{

}

public interface IDeliveryRecvPackage : IRecvPackage
{

}


public interface IDeliveryCallPackage<TSend, TRecv> : ICallPakcage<TSend, TRecv> where TSend : ISend where TRecv : IRecv, new()
{

}

public interface IGameDbSendPackage : ISendPakcage
{

}

public interface IGameDbRecvPackage : IRecvPackage
{

}

public interface IGameDbCallPackage<TSend, TRecv> : ICallPakcage<TSend, TRecv> where TSend : ISend where TRecv : IRecv, new()
{

}

public interface IPackageItem
{
    void PackTo(Packets packets);
}