// <summary>
// PwApiTest 顶层入口程序。
//
// 本程序是手工测试 Console 应用，并非 xUnit / NUnit 等单元测试框架；
// 通过手动注释 / 取消注释下方代码段来选择本次要执行的测试用例。
//
// 运行依赖：
//   - 真实的 DeliveryDB / GameDB 服务器在线（地址硬编码在
//     <see cref="PwApiTest.DeliveryDBTest"/> 与 <see cref="PwApiTest.GameDBTest"/> 中）；
//   - 本地无法离线实跑，只能在能连通游戏服务器的环境下观察控制台输出与游戏内表现。
// </summary>
using System;
using System.Threading.Tasks;
using PwApiTest;

Console.WriteLine("Hello, World!");




// 构造 DeliveryDB 测试客户端，构造函数内部会自动 Connect 到投递服务器
DeliveryDBTest deliveryDBTest = new();

// 等待 1 秒，确保 TCP 连接握手完成后再发送业务包
await Task.Delay(1000);
// 测试用例：发送一条公共聊天 / 世界喊话
deliveryDBTest.SendPublicChat("啦啦啦啦啦123<0><W><0:17><0><W><0:18><0><W><0:19>");


// 测试用例：通过系统邮件投递一件物品到指定角色
//await Task.Delay(1000);
//MailItem mailItem = new()
//{
//    Id = 12649,
//    Count = 1,
//    ProcType = 19,
//    Guid1 = 1509020271,
//    Guid2 = 16779733,
//    Mask = 1073741825,
//    Data = "5f004000320000002c01000048710000487100002c000000010000000d0000000e00000062210000ab040000ab08000000000000000000001c0000000000a0410000a04002003f006031000060310000060000008d25000004000000a521000002000000c52500000e00000086a400006400000086a4000064000000d1460000b00400000c000000"
//};
//deliveryDBTest.SendMail(64, "Title", "Content", 2000000000, mailItem);




// 测试用例：通过 GameDB 查询指定 UserId 名下的所有角色列表
//GameDBTest gameDB = new();

//await Task.Delay(1000);
//var roles = await gameDB.GetRolesAsync(64);
//foreach (var userRole in roles)
//{
//    Console.WriteLine(userRole.Key + ":" + userRole.Value);
//}



// 阻塞等待用户回车结束，方便观察控制台输出
Console.ReadLine();
