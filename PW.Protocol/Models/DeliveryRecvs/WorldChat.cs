namespace PW.Protocol.Models.DeliveryRecvs;

/// <summary>
/// 服务器主动推送的"世界喊话"包（包类型 0x85）。
/// 携带发言者角色 ID、昵称以及喊话内容，
/// 部分特殊频道（如 8 号"韦小宝重要物品喊话"）会用结构化数据替代纯文本。
/// </summary>
public class WorldChat : IRecvPackage
{
    /// <summary>包类型常量，固定为 0x85。</summary>
    public uint Type => 0x85u;

    /// <summary>原始字节流，由底层收包逻辑回填。</summary>
    public byte[] Data { get; set; }

    /// <summary>
    /// 频道编号（1 字节）。8 = 韦小宝重要物品喊话，其余为普通世界频道。
    /// </summary>
    public byte Channel { get; private set; }

    /// <summary>
    /// 表情编号（1 字节），用于聊天框前的表情图标。
    /// </summary>
    public byte Emotion { get; private set; }

    /// <summary>
    /// 来源角色 ID（4 字节整数）。
    /// </summary>
    public int SrcRoleId { get; private set; }

    /// <summary>
    /// 发言者昵称，从 Octets 段读取的字符串。
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// 喊话内容。普通频道为纯文本；
    /// 频道 8 时会被格式化为"&lt;角色名&gt; 获得了 &lt;物品 ID&gt;"。
    /// </summary>
    public string Message { get; private set; }

    /// <summary>
    /// 拆包：依次读取频道、表情、角色 ID、昵称，
    /// 再根据频道决定如何解析最后一段 Octets 的内容。
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
        // 变长 Octets：发言者昵称
        Name = up.UnPackOctets().GetString();

        // 取出消息体的 Octets，需要按频道判别其内部结构
        Octets o = up.UnPackOctets();
        if (Channel == 8) //韦小宝重要物品喊话
        {
            // 频道 8 是结构化数据：先 4 字节物品 ID，再字符串角色名
            int id = o.GetInt();
            string name = o.GetString();
            // 字符串末尾可能带 \0 截断，去掉以避免显示乱码
            name = name.TrimEnd('\0');
            Message = $"{name} 获得了 {id}";
        }
        else
        {
            // 普通频道：直接当作 UTF 字符串读取
            Message = o.GetString();
        }
    }

    /// <summary>
    /// 调试输出，列出所有字段。
    /// </summary>
    /// <returns>用于日志/控制台显示的字符串。</returns>
    public override string ToString()
    {
        return $"WorldChat---Channel={Channel},Emotion={Emotion},SrcRoleId={SrcRoleId},Name={Name},Message={Message}";
    }
}
