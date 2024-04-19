namespace PwApi.Models;

public class GetUserRoles : IGameDbCallPackage<GetUserRolesArg, GetUserRolesRes>
{
    public uint Type => 0xBD8u;

    public GetUserRolesArg Send { get; set; } = new();

    public GetUserRolesRes Recv { get; set; }
}

public class GetUserRolesArg : ISend
{
    public int UserId { get; set; }

    public void Pack(Packets packets)
    {
        packets.Pack(UserId);
    }
}

public class GetUserRolesRes : IRecv
{
    public int RetCode { get; private set; }

    //public IntListOctets Roles { get; private set; }

    public void UnPack(UnPackets p)
    {
        RetCode = p.UnPackInt();
        //Roles = p.UnPackOctets<IntListOctets>();

        // TODO
    }
}