namespace PW.Protocol.Models.GameDbCalls;

/// <summary>
/// <see cref="GetUserRoles"/> 的请求参数：按账号 ID 查询其名下所有角色。
/// </summary>
public record GetUserRolesArg : IPackTo
{
    /// <summary>要查询的账号（用户）ID。</summary>
    public int UserId { get; set; }

    /// <summary>
    /// 将查询条件按 GameDB 协议规则打包到发送缓冲区。
    /// </summary>
    /// <param name="packets">发送包写入器。</param>
    public void PackTo(SendPackets packets)
    {
        packets.Pack(UserId);
    }
}
