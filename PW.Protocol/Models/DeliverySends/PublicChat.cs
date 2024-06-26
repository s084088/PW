﻿namespace PW.Protocol.Models.DeliverySends;

public class PublicChat : ISendPackage
{
    public uint Type => 0x4Fu;

    public byte Cannel { get; set; } = 9;

    public byte Emotion { get; set; } = 0;

    public int RoleId { get; set; } = -1;

    public int LocalSid { get; set; } = 0;

    /// <summary>
    /// String
    /// </summary>
    public string Message { get; set; }

    public void PackTo(SendPackets packets)
    {
        packets.Pack(Cannel);
        packets.Pack(Emotion);
        packets.Pack(RoleId);
        packets.Pack(LocalSid);

        packets.Pack(new Octets().AddString(Message));
    }

    public override string ToString()
    {
        return $"Cannel={Cannel},Emotion={Emotion},RoleId={RoleId},LocalSid={LocalSid},Message={Message}";
    }
}