namespace PW.Protocol.Models.GameDbCalls;


/// <summary>
/// GameDB 调用包：按用户账号 ID 查询其名下所有角色列表。
/// 走同步请求-响应模式（<see cref="ICallPackage{TSend, TRecv}"/>），
/// 由 BaseClient 在 1 秒内匹配回应；超时静默返回 null。
/// 对应 GameDB 协议号 0xBD8。
/// </summary>
public class GetUserRoles : ICallPackage<GetUserRolesArg, GetUserRolesRes>
{
    /// <summary>协议包类型号（GameDB 端约定的命令字 0xBD8）。</summary>
    public uint Type => 0xBD8u;

    /// <summary>请求参数（待发送到 GameDB 的查询条件）。</summary>
    public GetUserRolesArg Send { get; set; } = new();

    /// <summary>服务器返回的响应包（含返回码与角色列表）。</summary>
    public GetUserRolesRes Recv { get; set; }
}
