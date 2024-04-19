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

    public List<GetUserRolesResRoleInfo> Roles { get; private set; } = [];

    public void UnPack(UnPackets p)
    {
        RetCode = p.UnPackInt();

        uint count = p.UnPackCUInt();

        for (int i = 0; i < count; i++)
        {
            GetUserRolesResRoleInfo role = new();
            role.UnPack(p);
            Roles.Add(role);
        }
    }
}


public class GetUserRolesResRoleInfo : IRecvPackageItem
{
    public int Id { get; set; }

    public StringOctets Name { get; set; }

    public void UnPack(UnPackets p)
    {
        Id = p.UnPackIntReverse();
        Name = p.UnPackOctets<StringOctets>();
    }
}