namespace PW.Protocol.Models.DeliveryRecvs;

public class WorldChat : IRecvPackage
{
    public uint Type => 0x85u;

    public byte Channel { get; private set; }

    public byte Emotion { get; private set; }

    public int SrcRoleId { get; private set; }

    /// <summary>
    /// string
    /// </summary>
    public Octets Name { get; private set; }

    /// <summary>
    /// string
    /// </summary>
    public Octets Message { get; private set; }

    public void UnPackFrom(RecvPackets up)
    {
        Channel = up.UnPackByte();
        Emotion = up.UnPackByte();
        SrcRoleId = up.UnPackInt();
        Name = up.UnPackOctets();
        Message = up.UnPackOctets();
    }

    public override string ToString()
    {
        return $"Channel={Channel},Emotion={Emotion},SrcRoleId={SrcRoleId},Name={Name.GetString()},Message={Message.GetString()}";
    }
}
