namespace PwApi.SendModels;

public class PublicChat : ISendPakcage
{
    public uint Type => 0x4Fu;

    public byte Cannel { get; set; } = 9;

    public byte Emotion { get; set; } = 0;

    public int RoleId { get; set; } = -1;

    public int LocalSid { get; set; } = 0;

    public StringOctets Message { get; set; }

    public Packets Pack()
    {
        Packets packets = new Packets();
        packets.Pack(Cannel);
        packets.Pack(Emotion);
        packets.Pack(RoleId);
        packets.Pack(LocalSid);
        packets.Pack(Message);

        return packets;
    }

    public override string ToString()
    {
        return $"Cannel={Cannel},Emotion={Emotion},RoleId={RoleId},LocalSid={LocalSid},Message={Message}";
    }
}