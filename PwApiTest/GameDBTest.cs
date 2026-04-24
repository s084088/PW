using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PW.Protocol.Comm;
using PW.Protocol.Models.GameDbCalls;

namespace PwApiTest;

/// <summary>
/// GameDB（游戏数据库服务器）的手工测试用例集合。
/// 构造时即建立到 GameDB 的 TCP 连接；
/// 各测试方法通过同步请求-响应模式调用 GameDB 的查询接口。
/// </summary>
internal class GameDBTest
{
    /// <summary>底层 GameDB 通讯客户端，承担收发包与同步请求-响应。</summary>
    private readonly BaseClient gameDB;


    /// <summary>
    /// 构造并连接到 GameDB（地址 / 端口硬编码，需要根据实际环境调整）。
    /// </summary>
    public GameDBTest()
    {
        gameDB = new();
        gameDB.Connect("192.168.200.100", 29400);
    }


    /// <summary>
    /// 测试目标：根据 UserId 查询其名下所有角色，返回 RoleId 到角色名的映射。
    /// 依赖服务器：GameDB。
    /// 预期表现：成功时控制台逐行打印 "&lt;RoleId&gt;:&lt;角色名&gt;"；
    /// 若服务器超过 1 秒未回包，BaseClient 会静默返回 null 致此处可能出现 NRE。
    /// </summary>
    /// <param name="id">要查询的玩家账号 UserId。</param>
    /// <returns>该账号名下的角色字典，键为 RoleId、值为角色名。</returns>
    public async Task<Dictionary<int, string>> GetRolesAsync(int id)
    {
        GetUserRoles getUserRoles = new();
        getUserRoles.Send.UserId = id;

        // 发送同步请求-响应，BaseClient 会等待对应类型的回包
        await gameDB.Send(getUserRoles);

        return getUserRoles.Recv.Roles.ToDictionary(x => x.Id, x => x.Name);

    }
}
