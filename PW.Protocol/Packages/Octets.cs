namespace PW.Protocol.Packages;

/// <summary>
/// 通用字节流容器（类似 Java 的 Octets / C++ 的 buffer）。
/// 内部用 <see cref="List{T}"/>&lt;<see cref="byte"/>&gt; 维护可增可删的字节序列，
/// 同时实现 <see cref="IPackTo"/> 与 <see cref="IUnPackFrom"/>，
/// 可作为子结构嵌入到 <see cref="SendPackets"/> / <see cref="RecvPackets"/> 中按
/// "长度前缀 + 原始字节" 的格式参与协议序列化。
/// </summary>
public class Octets : IPackTo, IUnPackFrom
{
    /// <summary>
    /// 内部底层字节缓冲，所有读写操作都直接作用于此列表。
    /// </summary>
    private List<byte> data = [];

    /// <summary>
    /// 构造一个空的字节流容器。
    /// </summary>
    public Octets() { }

    /// <summary>
    /// 用既有字节数组初始化容器，内部会复制一份。
    /// </summary>
    /// <param name="bytes">用于填充初始内容的字节数组。</param>
    public Octets(byte[] bytes) => data = [.. bytes];

    /// <summary>
    /// 向缓冲区追加或插入一段字节。
    /// </summary>
    /// <param name="b">待写入的字节数组。</param>
    /// <param name="pos">插入位置；为负数（默认 -1）时追加到末尾，否则在指定下标插入。</param>
    /// <returns>当前实例本身，便于链式调用。</returns>
    public Octets AddBytes(byte[] b, int pos = -1)
    {
        if (pos < 0)
            // 默认行为：直接追加到末尾
            data.AddRange(b);
        else
            // 指定下标：在 pos 位置插入，原数据后移
            data.InsertRange(pos, b);
        return this;
    }

    /// <summary>
    /// 写入单个字节。
    /// </summary>
    /// <param name="b">待写入的字节。</param>
    /// <param name="pos">插入位置；为负数（默认 -1）时追加到末尾。</param>
    /// <returns>当前实例本身，便于链式调用。</returns>
    public Octets AddByte(byte b, int pos = -1) => AddBytes([b], pos);

    /// <summary>
    /// 以 <see cref="Encoding.Unicode"/>（UTF-16 LE，每字符 2 字节）写入字符串原始字节。
    /// 注意：此方法不会写入长度前缀，需要调用方自行处理边界。
    /// </summary>
    /// <param name="s">待写入字符串。</param>
    /// <param name="pos">插入位置；为负数（默认 -1）时追加到末尾。</param>
    /// <returns>当前实例本身，便于链式调用。</returns>
    public Octets AddString(string s, int pos = -1) => AddBytes(Encoding.Unicode.GetBytes(s), pos);

    /// <summary>
    /// 将十六进制字符串（如 "A1B2C3"）解析为字节序列后写入缓冲区。
    /// 常用于把抓包得到的原始报文片段直接灌进发送数据。
    /// </summary>
    /// <param name="x">不带前缀、长度为偶数的十六进制字符串。</param>
    /// <param name="pos">插入位置；为负数（默认 -1）时追加到末尾。</param>
    /// <returns>当前实例本身，便于链式调用。</returns>
    public Octets AddHexString(string x, int pos = -1) => AddBytes(Convert.FromHexString(x), pos);

    /// <summary>
    /// 实现 <see cref="IPackTo"/>：把当前 Octets 序列化进发送包，
    /// 格式为 "CUInt 长度前缀 + 原始字节"。
    /// </summary>
    /// <param name="p">目标发送包。</param>
    public void PackTo(SendPackets p)
    {
        // 先写入变长无符号整型表示的字节数
        p.PackCUInt((uint)data.Count);
        // 再写入实际数据内容
        p.Pack([.. data]);
    }




    /// <summary>
    /// 实现 <see cref="IUnPackFrom"/>：从接收包中反序列化出 Octets 内容，
    /// 与 <see cref="PackTo"/> 的格式相对应（先 CUInt 长度，再字节正文）。
    /// </summary>
    /// <param name="p">来源接收包。</param>
    public void UnPackFrom(RecvPackets p)
    {
        // 先读出长度前缀
        uint size = p.UnPackCUInt();
        // 按长度读取数据填入内部缓冲
        data = p.UnPackBytes((int)size).ToList();
    }





    /// <summary>
    /// 从缓冲区"取出"一段字节（读取后会从内部 <see cref="data"/> 中移除，相当于消费）。
    /// </summary>
    /// <param name="length">读取长度；为负数（默认 -1）时读取从 <paramref name="pos"/> 起到末尾的全部字节。</param>
    /// <param name="pos">起始下标，默认从头开始。</param>
    /// <returns>读取到的字节数组。</returns>
    public byte[] GetBytes(int length = -1, int pos = 0)
    {
        // length 未指定时，读取剩余全部
        if (length < 0) length = data.Count - pos;
        // 截取目标区间
        byte[] bytes = data.Skip(pos).Take(length).ToArray();
        // 消费掉已读区间，模拟"流式读取"
        data.RemoveRange(pos, length);
        return bytes;
    }

    /// <summary>
    /// 从缓冲区头部消费 1 个字节。
    /// </summary>
    /// <returns>读取到的字节。</returns>
    public byte GetByte() => GetBytes(1, 0)[0];

    /// <summary>
    /// 把缓冲区中剩余全部字节按 <see cref="Encoding.Unicode"/>（UTF-16 LE）解码为字符串并消费。
    /// </summary>
    /// <returns>解码后的字符串。</returns>
    public string GetString() => Encoding.Unicode.GetString(GetBytes(-1, 0));

    /// <summary>
    /// 从缓冲区头部消费 4 个字节，按本机字节序解析为有符号 32 位整型。
    /// </summary>
    /// <returns>解析得到的 <see cref="int"/>。</returns>
    public int GetInt() => BitConverter.ToInt32(GetBytes(4, 0));




    /// <summary>
    /// 调试输出：以十六进制形式打印当前缓冲区内容。
    /// </summary>
    /// <returns>形如 "Octets---Data:XXXX" 的调试字符串。</returns>
    public override string ToString()
    {
        return $"Octets---Data:{data.ToArray().ToHexString()}";
    }
}
