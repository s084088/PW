namespace PW.Protocol.Models.DeliverySends;

public class SysSendMail : ISendPackage
{
    public uint Type => 0x1076u;

    public int TId { get; private set; } = IdHelper.GetId;
    public int SysId { get; private set; } = 32;
    public byte SysType { get; private set; } = 3;
    public int Receiver { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public GRoleInventory AttachObj { get; set; } = new();
    public int AttachMoney { get; set; }


    public void PackTo(SendPackets packets)
    {
        packets.Pack(TId);
        packets.Pack(SysId);
        packets.Pack(SysType);
        packets.Pack(Receiver);
        packets.Pack(new Octets().AddString(Title));
        packets.Pack(new Octets().AddString(Content));
        packets.Pack(AttachObj);
        packets.Pack(AttachMoney);
    }

    public override string ToString()
    {
        return $"TId={TId},SysId={SysId},SysType={SysType},Receiver={Receiver},Title={Title},Context={Content},AttachObj=({AttachObj}),AttachMoney={AttachMoney}";
    }
}


public class GRoleInventory : IPackTo
{
    public int Id { get; set; }

    public int Pos { get; private set; } = 0;

    public int Count { get; set; } = 1;

    public int MaxCount { get; set; } = 1;

    public string Data { get; set; } = string.Empty;

    public int ProcType { get; set; }

    public int ExpireDate { get; set; }

    public int Guid1 { get; set; }

    public int Guid2 { get; set; }

    public int Mask { get; set; }

    public void PackTo(SendPackets packets)
    {
        packets.Pack(Id);
        packets.Pack(Pos);
        packets.Pack(Count);
        packets.Pack(MaxCount);
        packets.Pack(new Octets().AddHexString(Data));
        packets.Pack(ProcType);
        packets.Pack(ExpireDate);
        packets.Pack(Guid1);
        packets.Pack(Guid2);
        packets.Pack(Mask);
    }

    public override string ToString()
    {
        return $"Id={Id},Count={Count},ProcType={ProcType},Mask={Mask},Data={Data}";
    }
}