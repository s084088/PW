namespace PW.Protocol.Models.DeliveryRecvs;

/// <summary>
/// 服务器主动推送的"聊天广播"包（包类型 0x78）。
/// 用于把某个频道（公告、世界等）的聊天内容广播给客户端，
/// 与 <see cref="WorldChat"/> 的区别在于本包不携带发言者昵称。
/// </summary>
public class ChatBroadCast : IRecvPackage
{
    /// <summary>包类型常量，固定为 0x78。</summary>
    public uint Type => 0x78u;

    /// <summary>原始字节流，由底层收包逻辑回填。</summary>
    public byte[] Data { get; set; }

    /// <summary>
    /// 频道编号（1 字节）。
    /// 9 = 公告（系统广播）。
    /// </summary>
    public byte Channel { get; private set; }

    /// <summary>
    /// 表情编号（1 字节），用于在聊天框前显示对应的表情图标。
    /// </summary>
    public byte Emotion { get; private set; }

    /// <summary>
    /// 来源角色 ID（4 字节整数），系统广播通常为 -1 或 0。
    /// </summary>
    public int SrcRoleId { get; private set; }

    /// <summary>
    /// 聊天文本内容，从 Octets 段中按字符串解析。
    /// </summary>
    public string Message { get; private set; }


    /// <summary>
    /// 拆包：依次读取频道、表情、来源角色 ID 与聊天文本。
    /// </summary>
    /// <param name="up">已经按包头切分好的接收包数据。</param>
    public void UnPackFrom(RecvPackets up)
    {
        // 1 字节频道
        Channel = up.UnPackByte();
        // 1 字节表情
        Emotion = up.UnPackByte();
        // 4 字节来源角色 ID
        SrcRoleId = up.UnPackInt();
        // 变长 Octets 转字符串：聊天文本
        Message = up.UnPackOctets().GetString();
    }

    /// <summary>
    /// 调试输出，列出所有字段。
    /// </summary>
    /// <returns>用于日志/控制台显示的字符串。</returns>
    public override string ToString()
    {
        return $"ChatBroadCast---Channel={Channel},Emotion={Emotion},SrcRoleId={SrcRoleId},Message={Message}";
    }
}
