namespace PW.Protocol.Models.DeliverySends;

/// <summary>
/// 世界喊话发包模型，对应 DeliveryDB 的世界频道公开喊话请求。
/// 由业务层（如 AutoAnnouncement 定时公告）构造后通过 BaseClient.Send 发送给服务端。
/// </summary>
public class PublicChat : ISendPackage
{
    /// <summary>协议包类型号，固定为 0x4F，DeliveryDB 据此识别为公开喊话请求。</summary>
    public uint Type => 0x4Fu;

    /// <summary>聊天频道编号，默认 9 表示世界频道。注意：字段名 Cannel 为历史拼写错误，保留以兼容协议命名。</summary>
    public byte Cannel { get; set; } = 9;

    /// <summary>表情编号，0 表示无表情，非零时客户端会显示对应的内嵌表情图标。</summary>
    public byte Emotion { get; set; } = 0;

    /// <summary>发送方角色 ID。-1 表示以系统名义发送（不绑定具体角色）。</summary>
    public int RoleId { get; set; } = -1;

    /// <summary>发送方所在分线/会话编号，0 表示无指定分线。</summary>
    public int LocalSid { get; set; } = 0;

    /// <summary>
    /// 喊话文本内容，会以 UTF-8 字节流附带长度前缀写入包体。
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 将当前对象按协议格式打包写入发送缓冲区。
    /// 字段顺序与服务端解析顺序严格对应：频道、表情、角色 ID、分线、文本。
    /// </summary>
    /// <param name="packets">目标发送缓冲区。</param>
    public void PackTo(SendPackets packets)
    {
        packets.Pack(Cannel);
        packets.Pack(Emotion);
        packets.Pack(RoleId);
        packets.Pack(LocalSid);

        // 文本字段需先包装成 Octets（带长度前缀的字节数组）再写入，符合服务端字符串协议
        packets.Pack(new Octets().AddString(Message));
    }

    /// <summary>
    /// 返回便于调试的字符串表示，列出所有字段当前值。
    /// </summary>
    /// <returns>字段名与值拼接的调试字符串。</returns>
    public override string ToString()
    {
        return $"Cannel={Cannel},Emotion={Emotion},RoleId={RoleId},LocalSid={LocalSid},Message={Message}";
    }
}
