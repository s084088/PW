using PwApi.Sockets;

namespace PwApi.Models;

public class GetUserRoles : IGameDbCallPackage<GetUserRolesArg, GetUserRolesRes>
{
    public uint Type => 0xBD8u;

    public GetUserRolesArg Send { get; set; } = new();

    public GetUserRolesRes Recv { get; set; }
}

public record GetUserRolesArg : ISend
{
    public int UserId { get; set; }

    public void Pack(Packets packets)
    {
        packets.Pack(UserId);
    }
}

public record GetUserRolesRes : IRecv
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

    public override string ToString()
    {
        return $"GetUserRolesRes {{ RetCode ={RetCode}, Roles = [ {string.Join(", ", Roles.Select(x => x.ToString()))} ] }}";
    }
}

public record GetUserRolesResRoleInfo : IRecvPackageItem
{
    public int Id { get; set; }

    public Octet Name { get; set; }

    public void UnPack(UnPackets p)
    {
        Id = p.UnPackIntReverse();
        Name = p.UnPackOctet();
    }
}