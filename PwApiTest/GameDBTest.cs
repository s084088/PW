using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PwApi;
using PwApi.Models;

namespace PwApiTest;

internal class GameDBTest
{
    private readonly GameDB gameDB;


    public GameDBTest()
    {
        gameDB = new("192.168.200.100", 29400);
    }


    public async Task<Dictionary<int, string>> GetRolesAsync(int id)
    {
        GetUserRoles getUserRoles = new();
        getUserRoles.Send.UserId = id;

        await gameDB.Send(getUserRoles);

        return getUserRoles.Recv.Roles.ToDictionary(x => x.Id, x => x.Name.GetString());

    }
}