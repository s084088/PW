namespace PW.Protocol.Models.DeliverySends;

/// <summary>
/// 系统邮件发包模型，对应 DeliveryDB 投递的官方/GM 邮件请求。
/// 支持携带文本正文、单件附件物品（GRoleInventory）以及附带金币。
/// </summary>
public class SysSendMail : ISendPackage
{
    /// <summary>协议包类型号，固定为 0x1076，DeliveryDB 据此识别为系统邮件投递请求。</summary>
    public uint Type => 0x1076u;

    /// <summary>事务 ID，由 IdHelper 自增分配，用于回包匹配请求。</summary>
    public int TId { get; private set; } = IdHelper.GetId;
    /// <summary>系统邮件来源标识，固定 32（约定的系统发件人编号）。</summary>
    public int SysId { get; private set; } = 32;
    /// <summary>系统邮件类别，固定 3（系统邮件类型）。</summary>
    public byte SysType { get; private set; } = 3;
    /// <summary>收件人角色 ID。</summary>
    public int Receiver { get; set; }
    /// <summary>邮件标题，写入时按字符串协议带长度前缀。</summary>
    public string Title { get; set; }
    /// <summary>邮件正文内容，写入时按字符串协议带长度前缀。</summary>
    public string Content { get; set; }
    /// <summary>附件物品对象。无附件时使用默认空实例（Id=0）。</summary>
    public GRoleInventory AttachObj { get; set; } = new();
    /// <summary>附件金币数量（单位：铜币）。</summary>
    public int AttachMoney { get; set; }


    /// <summary>
    /// 将系统邮件按协议格式打包写入发送缓冲区。
    /// 字段顺序：事务 ID、系统 ID、系统类型、收件人、标题、正文、附件物品、附件金币。
    /// </summary>
    /// <param name="packets">目标发送缓冲区。</param>
    public void PackTo(SendPackets packets)
    {
        packets.Pack(TId);
        packets.Pack(SysId);
        packets.Pack(SysType);
        packets.Pack(Receiver);
        // 标题与正文均需包装为 Octets，附带 UTF-8 字节流长度前缀
        packets.Pack(new Octets().AddString(Title));
        packets.Pack(new Octets().AddString(Content));
        // 附件物品由 GRoleInventory.PackTo 自行序列化各字段
        packets.Pack(AttachObj);
        packets.Pack(AttachMoney);
    }

    /// <summary>
    /// 返回便于调试的字符串表示，列出所有字段及附件物品摘要。
    /// </summary>
    /// <returns>字段名与值拼接的调试字符串。</returns>
    public override string ToString()
    {
        return $"TId={TId},SysId={SysId},SysType={SysType},Receiver={Receiver},Title={Title},Context={Content},AttachObj=({AttachObj}),AttachMoney={AttachMoney}";
    }
}


/// <summary>
/// 角色背包中的单件物品描述，用作系统邮件附件等场景的物品载体。
/// 实现 IPackTo 以便嵌入其他发包模型中按协议字段顺序序列化。
/// </summary>
public class GRoleInventory : IPackTo
{
    /// <summary>物品模板 ID（道具表主键），0 表示空附件。</summary>
    public int Id { get; set; }

    /// <summary>物品在背包中的位置编号，发送邮件附件场景下保持 0。</summary>
    public int Pos { get; private set; } = 0;

    /// <summary>物品堆叠数量，默认 1。</summary>
    public int Count { get; set; } = 1;

    /// <summary>该物品当前的最大可堆叠数量，默认 1。</summary>
    public int MaxCount { get; set; } = 1;

    /// <summary>
    /// 物品扩展数据（十六进制字符串），用于承载装备属性、强化、镶嵌等额外信息。
    /// 写入时通过 AddHexString 解析为字节数组再附带长度前缀。
    /// </summary>
    public string Data { get; set; } = string.Empty;

    /// <summary>处理类型标识，预留协议字段。</summary>
    public int ProcType { get; set; }

    /// <summary>物品过期时间（Unix 秒级时间戳），0 表示永不过期。</summary>
    public int ExpireDate { get; set; }

    /// <summary>物品全局唯一 ID 高位（部分物品如装备需要 GUID 标识）。</summary>
    public int Guid1 { get; set; }

    /// <summary>物品全局唯一 ID 低位。</summary>
    public int Guid2 { get; set; }

    /// <summary>物品掩码（绑定/锁定/活动来源等位标记）。</summary>
    public int Mask { get; set; }

    /// <summary>
    /// 将物品按协议格式打包写入发送缓冲区，字段顺序需与服务端反序列化一致。
    /// </summary>
    /// <param name="packets">目标发送缓冲区。</param>
    public void PackTo(SendPackets packets)
    {
        packets.Pack(Id);
        packets.Pack(Pos);
        packets.Pack(Count);
        packets.Pack(MaxCount);
        // Data 以十六进制字符串描述二进制扩展数据，需转换成字节数组再写入
        packets.Pack(new Octets().AddHexString(Data));
        packets.Pack(ProcType);
        packets.Pack(ExpireDate);
        packets.Pack(Guid1);
        packets.Pack(Guid2);
        packets.Pack(Mask);
    }

    /// <summary>
    /// 返回便于调试的字符串表示，仅列出关键字段。
    /// </summary>
    /// <returns>字段名与值拼接的调试字符串。</returns>
    public override string ToString()
    {
        return $"Id={Id},Count={Count},ProcType={ProcType},Mask={Mask},Data={Data}";
    }
}
