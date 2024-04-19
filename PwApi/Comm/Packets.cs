using System.ComponentModel;
using System.Drawing;

namespace PwApi.Comm;

public class Packets
{
    private readonly List<byte> data = [];

    public void Pack(int a)
    {
        byte[] d = BitConverter.GetBytes(a);
        Array.Reverse(d);
        data.AddRange(d);
    }

    public void PackCUInt(uint a)
    {
        data.AddRange(a.ToCUInt());
    }

    public void Pack(byte[] b)
    {
        data.AddRange(b);
    }

    public void Pack(byte b)
    {
        data.Add(b);
    }

    public void Pack(Octets o)
    {
        if (o == null) throw new Exception();
        data.AddRange(o.Size.ToCUInt());
        data.AddRange(o.Data);
    }

    public void Pack(ISendPackageItem item)
    {
        item.PackTo(this);
    }

    public byte[] GetBytes(uint type) => [.. type.ToCUInt(), .. ((uint)data.Count).ToCUInt(), .. data];
}

public class UnPackets
{
    private int po = 0;

    /// <summary>
    /// 包类型
    /// </summary>
    public uint Type { get; private set; }

    /// <summary>
    /// 包大小
    /// </summary>
    public uint Size { get; private set; }

    /// <summary>
    /// 包数据
    /// </summary>
    public byte[] Data { get; private set; }

    /// <summary>
    /// 未解析的数据
    /// </summary>
    public byte[] Unknown { get; private set; }

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
        uint r = Data.ReadCUInt(po, out int length);
        po += length;
        return r;
    }

    public T UnPackOctets<T>() where T : Octets, new()
    {
        T o = new();
        o.Size = UnPackCUInt();
        o.Data = UnPackBytes((int)o.Size);

        return o;
    }


    public UnPackets(byte[] package)
    {
        int pos = 0;
        Type = package.ReadCUInt(pos, out int length);
        pos += length;

        Size = package.ReadCUInt(pos, out length);
        pos += length;

        Data = new byte[Size];
        Array.Copy(package, pos, Data, 0, Size);
        pos += (int)Size;

        if (pos < package.Length)
        {
            Unknown = new byte[package.Length - pos];
            Array.Copy(package, pos, Unknown, 0, Unknown.Length);
        }
        else
        {
            Unknown = [];
        }
    }

    public override string ToString()
    {
        return $"type={Type:X},size={Size:X},data={Data.ToHexString()}";
    }
}