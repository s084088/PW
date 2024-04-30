using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PW.Protocol.Comm;
using PW.Protocol.Models.GameDbCalls;

namespace PwApiTest;

internal class GameDBTest
{
    private readonly BaseClient gameDB;


    public GameDBTest()
    {
        gameDB = new();
        gameDB.Connect("192.168.200.100", 29400);
    }


    public async Task<Dictionary<int, string>> GetRolesAsync(int id)
    {
        GetUserRoles getUserRoles = new();
        getUserRoles.Send.UserId = id;

        await gameDB.Send(getUserRoles);

        return getUserRoles.Recv.Roles.ToDictionary(x => x.Id, x => x.Name);

    }
}