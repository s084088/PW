namespace PW.Protocol.Packages;

/// <summary>
/// 发送包基类：用于按协议格式 "CUInt Type + CUInt Size + Size 字节 Data" 组装一条上行报文。
/// 通过主构造参数指定包类型，业务代码再依次调用各 <c>Pack</c> 方法把字段写入 <see cref="Data"/>。
/// 最终通过 <see cref="GetBytes"/> 输出可直接写入 socket 的字节流。
/// </summary>
/// <param name="type">包类型编号，对应游戏服务器协议中的 opcode。</param>
public class SendPackets(uint type)
{
    /// <summary>
    /// 包正文缓冲（不含包头）。所有 <c>Pack</c> 方法都会向此列表追加字节。
    /// </summary>
    public List<byte> Data { get; init; } = [];

    /// <summary>
    /// 包类型编号；构造时给定后不再变化。
    /// </summary>
    public uint Type { get; init; } = type;

    /// <summary>
    /// 写入 1 个 32 位有符号整型，按<b>大端（网络字节序）</b>编码。
    /// </summary>
    /// <param name="a">待写入的整数。</param>
    public void Pack(int a)
    {
        byte[] d = BitConverter.GetBytes(a);
        // BitConverter 默认按本机字节序输出，这里反转为大端
        Array.Reverse(d);
        Data.AddRange(d);
    }

    /// <summary>
    /// 写入 1 个 32 位无符号整型，使用变长 CUInt 紧凑编码。
    /// </summary>
    /// <param name="a">待写入的整数。</param>
    public void PackCUInt(uint a)
    {
        Data.AddRange(a.ToCUInt());
    }

    /// <summary>
    /// 原样追加一段字节，不做任何字节序处理。
    /// </summary>
    /// <param name="b">待追加的字节数组。</param>
    public void Pack(byte[] b)
    {
        Data.AddRange(b);
    }

    /// <summary>
    /// 追加 1 个字节。
    /// </summary>
    /// <param name="b">待追加的字节。</param>
    public void Pack(byte b)
    {
        Data.Add(b);
    }

    /// <summary>
    /// 通用打包入口：让实现了 <see cref="IPackTo"/> 的对象自行把字段写入当前包。
    /// </summary>
    /// <param name="item">可自序列化的子结构。</param>
    public void Pack(IPackTo item)
    {
        item.PackTo(this);
    }

    /// <summary>
    /// 按协议格式输出最终字节流："CUInt Type + CUInt Size + Size 字节 Data"，
    /// 可直接写入 socket 发送。
    /// </summary>
    /// <returns>完整的待发送字节数组。</returns>
    public byte[] GetBytes() => [.. Type.ToCUInt(), .. ((uint)Data.Count).ToCUInt(), .. Data];


    /// <summary>
    /// 调试输出：以十六进制形式打印包类型与正文。
    /// </summary>
    /// <returns>形如 "SendPackage---Type:N--Data:XXXX" 的调试字符串。</returns>
    public override string ToString()
    {
        return $"SendPackage---Type:{Type}--Data:{Data.ToArray().ToHexString()}";
    }
}
