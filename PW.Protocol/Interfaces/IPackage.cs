using PW.Protocol.Packages;

namespace PW.Protocol.Interfaces;

public interface IPackage
{
    uint Type { get; }
}


public interface ISendPackage : IPackage, IPackTo
{

}

public interface IRecvPackage : IPackage, IUnPackFrom
{
    byte[] Data { get; internal set; }
}

public interface ICallPackage<TSend, TRecv> : IPackage where TSend : IPackTo where TRecv : IUnPackFrom
{
    TSend Send { get; set; }

    TRecv Recv { get; set; }
}