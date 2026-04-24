using System.Net;

namespace PW.Protocol.Packages;

/// <summary>
/// 接收包基类：表示一条从服务器读到的、已完成"包头解析"的原始包。
/// 报文整体格式为 "CUInt Type + CUInt Size + Size 字节 Data"。
/// 内部维护一个游标 <see cref="po"/>，所有 <c>UnPackXxx</c> 方法都会顺序前移。
/// </summary>
public class RecvPackets
{
    /// <summary>
    /// 内部读取游标（指向 <see cref="Data"/> 中下一个待读字节的位置）。
    /// </summary>
    private int po = 0;

    /// <summary>
    /// 包类型 0为解析失败
    /// </summary>
    public uint Type { get; init; }

    /// <summary>
    /// 包大小
    /// </summary>
    public uint Size { get; init; }

    /// <summary>
    /// 包数据
    /// </summary>
    public byte[] Data { get; init; }

    /// <summary>
    /// 从当前游标位置读取 1 字节，并将游标后移 1。
    /// </summary>
    /// <returns>读取到的字节。</returns>
    public byte UnPackByte()
    {
        byte b = Data[po];
        po++;
        return b;
    }

    /// <summary>
    /// 从当前游标位置读取指定长度的字节数组，并将游标相应后移。
    /// </summary>
    /// <param name="length">需要读取的字节数。</param>
    /// <returns>读取到的字节数组。</returns>
    public byte[] UnPackBytes(int length)
    {
        byte[] b = new byte[length];
        // 从底层 Data 复制 length 字节到新数组
        Array.Copy(Data, po, b, 0, length);
        po += length;
        return b;
    }

    /// <summary>
    /// 从当前游标位置读取 4 字节并按 <b>大端（网络字节序）</b>解析为有符号 32 位整型。
    /// </summary>
    /// <returns>解析得到的 <see cref="int"/>。</returns>
    public int UnPackInt()
    {
        int i = BitConverter.ToInt32(Data, po);
        po += 4;
        // 将网络字节序转换为本机字节序
        return IPAddress.NetworkToHostOrder(i);
    }

    /// <summary>
    /// 从当前游标位置读取 4 字节并以"反转字节序"的方式解析为 <see cref="int"/>。
    /// 适用于服务器以小端发送、本机也是小端时的特殊翻转协议。
    /// </summary>
    /// <returns>解析得到的 <see cref="int"/>。</returns>
    public int UnPackIntReverse()
    {
        byte[] i = UnPackBytes(4);
        // 把读出来的 4 字节就地反转
        Array.Reverse(i);
        return BitConverter.ToInt32(i);
    }

    /// <summary>
    /// 从当前游标位置读取一个变长无符号整型（CUInt，紧凑编码）。
    /// 实际编码规则由扩展方法 <c>ReadCUInt</c> 决定。
    /// </summary>
    /// <returns>解析得到的 <see cref="uint"/>。</returns>
    public uint UnPackCUInt()
    {
        uint r = Data.ReadCUInt(ref po);
        return r;
    }

    /// <summary>
    /// 从当前游标位置反序列化出一个 <see cref="Octets"/> 子结构（"CUInt 长度 + 字节正文"）。
    /// </summary>
    /// <returns>新构造并已填充内容的 <see cref="Octets"/>。</returns>
    public Octets UnPackOctets()
    {
        Octets octets = new();
        UnPack(octets);
        return octets;
    }

    /// <summary>
    /// 通用反序列化入口：让目标对象从当前包里自行读出自己的字段。
    /// </summary>
    /// <param name="unPack">实现了 <see cref="IUnPackFrom"/> 的目标对象。</param>
    public void UnPack(IUnPackFrom unPack)
    {
        unPack.UnPackFrom(this);
    }


    /// <summary>
    /// 将一段可能包含多条粘包数据的字节流，按协议格式拆分为若干 <see cref="RecvPackets"/>。
    /// 解析失败时也会把剩余数据封装为一条"裸数据"包后结束循环（详见 <see cref="GetPacket"/>）。
    /// </summary>
    /// <param name="package">原始字节流，可能包含 0 ~ N 条完整包。</param>
    /// <returns>按顺序拆分得到的包列表。</returns>
    public static List<RecvPackets> GetPackets(byte[] package)
    {
        List<RecvPackets> list = [];
        // 循环切包，直到剩余字节耗尽或解析失败返回 null
        while (package?.Length > 0)
        {
            (RecvPackets packets, byte[] remainingPackage) = GetPacket(package);
            list.Add(packets);
            package = remainingPackage;
        }

        return list;
    }

    /// <summary>
    /// 从字节流头部提取一条完整包，并返回剩余未消费部分。
    /// 任何异常都会被吞掉，并把剩余原始数据整段塞进一条裸包里返回，
    /// 同时把剩余数组置为 <c>null</c> 以终止外层 <see cref="GetPackets"/> 的循环。
    /// </summary>
    /// <param name="package">待解析的字节流。</param>
    /// <returns>形如 (一条包, 剩余字节流) 的元组；解析失败时第二项为 <c>null</c>。</returns>
    private static (RecvPackets, byte[]) GetPacket(byte[] package)
    {
        try
        {
            int pos = 0;

            // 1) 读取 CUInt 包类型
            uint type = package.ReadCUInt(ref pos);

            // 2) 读取 CUInt 包体长度
            uint size = package.ReadCUInt(ref pos);

            // 3) 按长度截取包体数据
            byte[] data = new byte[size];
            Array.Copy(package, pos, data, 0, size);
            pos += (int)size;

            RecvPackets packets = new()
            {
                Type = type,
                Size = size,
                Data = data
            };

            // 4) 剩余字节即为下一条包的起始
            byte[] remainingPackage = package.Skip(pos).ToArray();

            return (packets, remainingPackage);
        }
        catch
        {
            // 解析失败：把剩余字节作为裸数据包返回，并以 null 通知外层停止循环
            RecvPackets packets = new() { Data = package };
            return (packets, null);
        }
    }


    /// <summary>
    /// 调试输出：以十六进制形式打印包类型与正文。
    /// </summary>
    /// <returns>形如 "SendPackage---Type:N--Data:XXXX" 的调试字符串。</returns>
    public override string ToString()
    {
        return $"SendPackage---Type:{Type}--Data:{Data.ToArray().ToHexString()}";
    }
}
