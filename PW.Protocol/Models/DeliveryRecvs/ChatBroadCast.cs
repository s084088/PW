﻿namespace PW.Protocol.Models.DeliveryRecvs;

public class ChatBroadCast : IRecvPackage
{
    public uint Type => 0x78u;

    /// <summary>
    /// 频道
    /// 9 公告
    /// </summary>
    public byte Channel { get; private set; }

    public byte Emotion { get; private set; }

    public int SrcRoleId { get; private set; }

    /// <summary>
    /// 信息
    /// 9 string
    /// </summary>
    public Octets Message { get; private set; }

    public void UnPackFrom(RecvPackets up)
    {
        Channel = up.UnPackByte();
        Emotion = up.UnPackByte();
        SrcRoleId = up.UnPackInt();
        Message = up.UnPackOctets();
    }

    public override string ToString()
    {
        return $"Channel={Channel},Emotion={Emotion},SrcRoleId={SrcRoleId},Message={Message.GetString()}";
    }
}