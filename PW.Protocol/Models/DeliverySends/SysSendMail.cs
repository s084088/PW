namespace PW.Protocol.Models.DeliverySends;

public class SysSendMail : ISendPackage
{
    public uint Type => 0x1076u;

    public int TId { get; private set; } = IdHelper.GetId;
    public int SysId { get; private set; } = 32;
    public byte SysType { get; private set; } = 3;
    public int Receiver { get; set; }

    /// <summary>
    /// string
    /// </summary>
    public Octets Title { get; set; } = new();

    /// <summary>
    /// string
    /// </summary>
    public Octets Content { get; set; } = new();
    public GRoleInventory AttachObj { get; set; } = new();
    public int AttachMoney { get; set; }


    public void PackTo(SendPackets packets)
    {
        packets.Pack(TId);
        packets.Pack(SysId);
        packets.Pack(SysType);
        packets.Pack(Receiver);
        packets.Pack(Title);
        packets.Pack(Content);
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

    /// <summary>
    /// hexString
    /// </summary>
    public Octets Data { get; set; } = new();

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
        packets.Pack(Data);
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