namespace PwApi.Models;

public class ChatBroadCast : IDeliveryRecvPackage
{
    public uint Type => 0x78u;

    public byte Channel { get; private set; }

    public byte Emotion { get; private set; }

    public int SrcRoleId { get; private set; }

    public StringOctets Message { get; private set; }

    public void UnPack(UnPackets up)
    {
        Channel = up.UnPackByte();
        Emotion = up.UnPackByte();
        SrcRoleId = up.UnPackInt();
        Message = up.UnPackOctets<StringOctets>();
    }

    public override string ToString()
    {
        return $"Channel={Channel},Emotion={Emotion},SrcRoleId={SrcRoleId},Message={Message}";
    }
}