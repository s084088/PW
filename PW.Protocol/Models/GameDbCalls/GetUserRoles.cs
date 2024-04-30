namespace PW.Protocol.Models.GameDbCalls;


public class GetUserRoles : ICallPackage<GetUserRolesArg, GetUserRolesRes>
{
    public uint Type => 0xBD8u;

    public GetUserRolesArg Send { get; set; } = new();

    public GetUserRolesRes Recv { get; set; }
}