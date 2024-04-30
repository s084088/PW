namespace PW.Protocol.Models.GameDbCalls;

public record GetUserRolesRes : IUnPackFrom
{
    public int RetCode { get; private set; }

    public List<GetUserRolesResRoleInfo> Roles { get; private set; } = [];

    public void UnPackFrom(RecvPackets p)
    {
        RetCode = p.UnPackInt();

        uint count = p.UnPackCUInt();

        for (int i = 0; i < count; i++)
        {
            GetUserRolesResRoleInfo role = new();
            role.UnPackFrom(p);
            Roles.Add(role);
        }
    }

    public override string ToString()
    {
        return $"GetUserRolesRes {{ RetCode ={RetCode}, Roles = [ {string.Join(", ", Roles.Select(x => x.ToString()))} ] }}";
    }
}

public record GetUserRolesResRoleInfo : IUnPackFrom
{
    public int Id { get; set; }

    public Octets Name { get; set; }

    public void UnPackFrom(RecvPackets p)
    {
        Id = p.UnPackIntReverse();
        Name = p.UnPackOctets();
    }
}