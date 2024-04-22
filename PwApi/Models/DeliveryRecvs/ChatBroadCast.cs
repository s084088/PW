namespace PwApi.Models;

public class ChatBroadCast : IDeliveryRecvPackage
{
    public uint Type => 0x78u;

    /// <summary>
    /// 频道
    /// 8 韦小宝
    /// 9 公告
    /// </summary>
    public byte Channel { get; private set; }

    public byte Emotion { get; private set; }

    public int SrcRoleId { get; private set; }

    /// <summary>
    /// 信息
    /// 8 int + string
    /// 9 string
    /// </summary>
    public Octet Message { get; private set; }

    public void UnPack(UnPackets up)
    {
        Channel = up.UnPackByte();
        Emotion = up.UnPackByte();
        SrcRoleId = up.UnPackInt();
        Message = up.UnPackOctet();
    }

    public override string ToString()
    {
        return $"Channel={Channel},Emotion={Emotion},SrcRoleId={SrcRoleId},Message={Message}";
    }
}