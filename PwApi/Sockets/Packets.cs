namespace PwApi.Sockets;

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

    public void Pack(Octet o)
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