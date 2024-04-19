namespace PwApi.Models.DeliveryRecvs;

public class WorldChat : IDeliveryRecvPackage
{
    public uint Type => 0x85u;

    public byte Channel { get; private set; }

    public byte Emotion { get; private set; }

    public int SrcRoleId { get; private set; }

    public StringOctets Name { get; private set; }

    public StringOctets Message { get; private set; }

    public void UnPack(UnPackets up)
    {
        Channel = up.UnPackByte();
        Emotion = up.UnPackByte();
        SrcRoleId = up.UnPackInt();
        Name = up.UnPackOctets<StringOctets>();
        Message = up.UnPackOctets<StringOctets>();
    }

    public override string ToString()
    {
        return $"Channel={Channel},Emotion={Emotion},SrcRoleId={SrcRoleId},Name={Name},Message={Message}";
    }
}
