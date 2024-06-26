﻿namespace PW.Protocol.Packages;

public class Octets : IPackTo, IUnPackFrom
{
    private List<byte> data = [];

    public Octets() { }

    public Octets(byte[] bytes) => data = [.. bytes];

    public Octets AddBytes(byte[] b, int pos = -1)
    {
        if (pos < 0)
            data.AddRange(b);
        else
            data.InsertRange(pos, b);
        return this;
    }

    public Octets AddByte(byte b, int pos = -1) => AddBytes([b], pos);

    public Octets AddString(string s, int pos = -1) => AddBytes(Encoding.Unicode.GetBytes(s), pos);

    public Octets AddHexString(string x, int pos = -1) => AddBytes(Convert.FromHexString(x), pos);

    public void PackTo(SendPackets p)
    {
        p.PackCUInt((uint)data.Count);
        p.Pack([.. data]);
    }




    public void UnPackFrom(RecvPackets p)
    {
        uint size = p.UnPackCUInt();
        data = p.UnPackBytes((int)size).ToList();
    }





    public byte[] GetBytes(int length = -1, int pos = 0)
    {
        if (length < 0) length = data.Count - pos;
        byte[] bytes = data.Skip(pos).Take(length).ToArray();
        data.RemoveRange(pos, length);
        return bytes;
    }

    public byte GetByte() => GetBytes(1, 0)[0];

    public string GetString() => Encoding.Unicode.GetString(GetBytes(-1, 0));

    public int GetInt() => BitConverter.ToInt32(GetBytes(4, 0));




    public override string ToString()
    {
        return $"Octets---Data:{data.ToArray().ToHexString()}";
    }
}