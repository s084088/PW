namespace PW.Protocol.Packages;
public class RecvPackets
{
    private int po = 0;

    /// <summary>
    /// 包类型
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

    public byte UnPackByte()
    {
        byte b = Data[po];
        po++;
        return b;
    }

    public byte[] UnPackBytes(int length)
    {
        byte[] b = new byte[length];
        Array.Copy(Data, po, b, 0, length);
        po += length;
        return b;
    }

    public int UnPackInt()
    {
        int i = BitConverter.ToInt32(Data, po);
        po += 4;
        return i;
    }

    public int UnPackIntReverse()
    {
        byte[] i = UnPackBytes(4);
        Array.Reverse(i);
        return BitConverter.ToInt32(i);
    }

    public uint UnPackCUInt()
    {
        uint r = Data.ReadCUInt(ref po);
        return r;
    }

    public void UnPackOctet(IUnPackFrom unPack)
    {
        unPack.UnPackFrom(this);
    }




    public static List<RecvPackets> GetPackets(byte[] package)
    {
        List<RecvPackets> list = [];
        while (package?.Length > 0)
        {
            (RecvPackets packets, byte[] remainingPackage) = GetPacket(package);
            list.Add(packets);
            package = remainingPackage;
        }

        return list;
    }

    private static (RecvPackets, byte[]) GetPacket(byte[] package)
    {
        int pos = 0;

        uint type = package.ReadCUInt(ref pos);

        uint size = package.ReadCUInt(ref pos);

        byte[] data = new byte[size];
        Array.Copy(package, pos, data, 0, size);
        pos += (int)size;

        RecvPackets packets = new()
        {
            Type = type,
            Size = size,
            Data = data
        };

        byte[] remainingPackage = package.Skip(pos).ToArray();

        return (packets, remainingPackage);
    }


    public override string ToString()
    {
        return $"SendPackage---Type:{Type}--Data:{Data.ToArray().ToHexString()}";
    }
}