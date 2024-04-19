namespace PwApi.Models;

public class AnnounceServerAttribute : IDeliveryRecvPackage
{
    public uint Type => 0x84u;

    public int Attr { get; private set; }

    public int FreeCreatTime { get; private set; }

    public void UnPack(UnPackets up)
    {
        Attr = up.UnPackInt();
        FreeCreatTime = up.UnPackInt();
    }

    public override string ToString()
    {
        return $"Attr={Attr},FreeCreatTime={FreeCreatTime}";
    }
}
