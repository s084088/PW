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
    }

    public string GetString()
    {
        if (Data == null) return null;
        return Encoding.Unicode.GetString(Data);
    }

    public StringOctets() { }

    public StringOctets(string s) => SetString(s);

    public static implicit operator StringOctets(string s) => new(s);

    public static implicit operator string(StringOctets s) => s.GetString();
}

public class XmlOctets : Octets
{
    public void SetXml(string s)
    {
        if (s == null) return;
        Data = Convert.FromHexString(s);
    }

    public string GetXml()
    {
        if (Data == null) return null;
        return Convert.ToHexString(Data);
    }

    public XmlOctets() { }

    public XmlOctets(string s) => SetXml(s);

    public static implicit operator XmlOctets(string s) => new(s);

    public static implicit operator string(XmlOctets s) => s.GetXml();
}

