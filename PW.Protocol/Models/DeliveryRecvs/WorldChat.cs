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

        Octets o = up.UnPackOctets();
        if (Channel == 8) //韦小宝重要物品喊话
        {
            int id = o.GetInt();
            string name = o.GetString();
            name = name.TrimEnd('\0');
            Message = $"{name} 获得了 {id}";
        }
        else
        {
            Message = o.GetString();
        }
    }

    public override string ToString()
    {
        return $"WorldChat---Channel={Channel},Emotion={Emotion},SrcRoleId={SrcRoleId},Name={Name},Message={Message}";
    }
}
