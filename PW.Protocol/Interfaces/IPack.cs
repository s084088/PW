using PW.Protocol.Packages;

namespace PW.Protocol.Interfaces;

public interface IPackTo
{
     void PackTo(SendPackets p);
}

public interface IUnPackFrom
{
    void UnPackFrom(RecvPackets p);
}