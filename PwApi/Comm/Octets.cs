namespace PwApi.Comm;

public abstract class Octets
{
    public uint Size { get; set; }

    public byte[] Data { get; set; }
}

public class StringOctets : Octets
{
    public void SetString(string s)
    {
        if (s == null) return;
        Data = Encoding.Unicode.GetBytes(s);
        Size = (uint)Data.Length;
    }

    public string GetString()
    {
        if (Data == null) return null;
        return Encoding.Unicode.GetString(Data);
    }

    public StringOctets() { }

    public StringOctets(string s) => SetString(s);

    public override string ToString() => GetString();

    public static implicit operator StringOctets(string s) => new(s);

    public static implicit operator string(StringOctets s) => s.GetString();
}

public class XmlOctets : Octets
{
    public void SetXml(string s)
    {
        if (s == null) return;
        Data = Convert.FromHexString(s);
        Size = (uint)Data.Length;
    }

    public string GetXml()
    {
        if (Data == null) return null;
        return Convert.ToHexString(Data);
    }

    public XmlOctets() { }

    public XmlOctets(string s) => SetXml(s);

    public override string ToString() => GetXml();

    public static implicit operator XmlOctets(string s) => new(s);

    public static implicit operator string(XmlOctets s) => s.GetXml();
}

public class IntListOctets : Octets
{
    public void SetInts(IEnumerable<int> list)
    {
        if (list == null) return;

        List<byte> bytes = [];
        foreach (int i in list)
        {
            byte[] bs = BitConverter.GetBytes(i);
            bytes.AddRange(bs);
        }
        Data = bytes.ToArray();
        Size = (uint)Data.Length;
    }

    public int[] GetInts()
    {
        List<int> ints = [];

        for (int i = 0; i < Data.Length; i += 4)
        {
            int data = BitConverter.ToInt32(Data, i);
            ints.Add(data);
        }
        return [.. ints];
    }
}