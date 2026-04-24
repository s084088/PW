namespace PW.Protocol.Models.GameDbCalls;

/// <summary>
/// <see cref="GetUserRoles"/> 的响应包：含返回码与该账号下的角色列表。
/// </summary>
public record GetUserRolesRes : IUnPackFrom
{
    /// <summary>GameDB 返回码（0 通常表示成功，其他值为错误码）。</summary>
    public int RetCode { get; private set; }

    /// <summary>账号名下的角色列表（每项为一个角色摘要信息）。</summary>
    public List<GetUserRolesResRoleInfo> Roles { get; private set; } = [];

    /// <summary>
    /// 按 GameDB 协议规则从接收缓冲区解析响应：先读返回码，再按变长 uint 读取角色数量，逐项拆包。
    /// </summary>
    /// <param name="p">接收包读取器。</param>
    public void UnPackFrom(RecvPackets p)
    {
        RetCode = p.UnPackInt();

        // 读取角色数量（变长无符号整数 CUInt）
        uint count = p.UnPackCUInt();

        // 按数量循环拆出每个角色信息
        for (int i = 0; i < count; i++)
        {
            GetUserRolesResRoleInfo role = new();
            role.UnPackFrom(p);
            Roles.Add(role);
        }
    }

    /// <summary>
    /// 输出便于调试的字符串表示。
    /// </summary>
    /// <returns>包含返回码与所有角色信息的可读字符串。</returns>
    public override string ToString()
    {
        return $"GetUserRolesRes {{ RetCode ={RetCode}, Roles = [ {string.Join(", ", Roles.Select(x => x.ToString()))} ] }}";
    }
}

/// <summary>
/// 角色摘要信息：<see cref="GetUserRolesRes.Roles"/> 列表中的一项，描述账号下的单个角色。
/// </summary>
public record GetUserRolesResRoleInfo : IUnPackFrom
{
    /// <summary>角色 ID（roleId，游戏内唯一标识）。</summary>
    public int Id { get; set; }

    /// <summary>角色名（UTF-8 字符串）。</summary>
    public string Name { get; set; }

    /// <summary>
    /// 按 GameDB 协议拆出单个角色信息：先读小端反序的角色 ID，再读字节串形式的角色名。
    /// </summary>
    /// <param name="p">接收包读取器。</param>
    public void UnPackFrom(RecvPackets p)
    {
        Id = p.UnPackIntReverse();
        Name = p.UnPackOctets().GetString();
    }
}
