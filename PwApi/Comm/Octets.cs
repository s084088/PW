namespace PwApi.Comm;

public class Octet
{
    public uint Size => (uint)Data.Count;

    public List<byte> Data { get; set; } = [];






    public void AddBytes(byte[] data, int pos = -1)
    {
        if (pos < 0)
            Data.AddRange(data);
        else
            Data.InsertRange(pos, data);
    }

    public void AddByte(byte b, int pos = -1) => AddBytes([b], pos);

    public void AddString(string s, int pos = -1) => AddBytes(Encoding.Unicode.GetBytes(s), pos);

    public void AddHexString(string x, int pos = -1) => AddBytes(Convert.FromHexString(x), pos);








    public byte[] GetBytes(int length = -1, int pos = 0)
    {
        if (length < 0) length = Data.Count - pos;
        byte[] bytes = Data.Skip(pos).Take(length).ToArray();
        Data.RemoveRange(pos, bytes.Length);
        return bytes;
    }

    public byte GetByte(int pos) => GetBytes(1, pos)[0];

    public string GetString(int length = -1, int pos = 0) => Encoding.Unicode.GetString(GetBytes(length, pos));

    public string GetHexString(int length = -1, int pos = 0) => Convert.ToHexString(GetBytes(length, pos));


    public override string ToString() => Convert.ToHexString(Data.ToArray());
}