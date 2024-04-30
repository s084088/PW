namespace PW.Protocol.Packages;

public class SendPackets(uint type)
{
    public List<byte> Data { get; init; } = [];

    public uint Type { get; init; } = type;

    public void Pack(int a)
    {
        byte[] d = BitConverter.GetBytes(a);
        Array.Reverse(d);
        Data.AddRange(d);
    }

    public void PackCUInt(uint a)
    {
        Data.AddRange(a.ToCUInt());
    }

    public void Pack(byte[] b)
    {
        Data.AddRange(b);
    }

    public void Pack(byte b)
    {
        Data.Add(b);
    }

    public void Pack(IPackTo item)
    {
        item.PackTo(this);
    }

    public byte[] GetBytes() => [.. Type.ToCUInt(), .. ((uint)Data.Count).ToCUInt(), .. Data];


    public override string ToString()
    {
        return $"SendPackage---Type:{Type}--Data:{Data.ToArray().ToHexString()}";
    }
}
