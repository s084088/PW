﻿namespace PW.Protocol.Models.DeliveryRecvs;

public class WorldChat : IRecvPackage
{
    public uint Type => 0x85u;
    public byte[] Data { get; set; }

    public byte Channel { get; private set; }

    public byte Emotion { get; private set; }

    public int SrcRoleId { get; private set; }

    public string Name { get; private set; }

    public string Message { get; private set; }

    public void UnPackFrom(RecvPackets up)
    {
        Channel = up.UnPackByte();
        Emotion = up.UnPackByte();
        SrcRoleId = up.UnPackInt();
        Name = up.UnPackOctets().GetString();
        Message = up.UnPackOctets().GetString();
    }

    public override string ToString()
    {
        return $"WorldChat---Channel={Channel},Emotion={Emotion},SrcRoleId={SrcRoleId},Name={Name},Message={Message}";
    }
}
