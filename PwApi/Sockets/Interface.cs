namespace PwApi.Sockets;

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





public interface ISendPackage : IPackage, ISend
{

}

public interface IRecvPackage : IPackage, IRecv
{
}

public interface ICallPackage<TSend, TRecv> : IPackage where TSend : ISend where TRecv : IRecv
{
    TSend Send { get; set; }

    TRecv Recv { get; set; }
}



public interface IDeliverySendPackage : ISendPackage
{

}

public interface IDeliveryRecvPackage : IRecvPackage
{

}


public interface IDeliveryCallPackage<TSend, TRecv> : ICallPackage<TSend, TRecv> where TSend : ISend where TRecv : IRecv
{

}

public interface IGameDbSendPackage : ISendPackage
{

}

public interface IGameDbRecvPackage : IRecvPackage
{

}

public interface IGameDbCallPackage<TSend, TRecv> : ICallPackage<TSend, TRecv> where TSend : ISend where TRecv : IRecv
{

}

public interface ISendPackageItem
{
    void PackTo(Packets p);
}


public interface IRecvPackageItem
{
    void UnPack(UnPackets p);
}