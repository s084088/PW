// See https://aka.ms/new-console-template for more information
using System;
using System.Threading.Tasks;
using PwApiTest;

Console.WriteLine("Hello, World!");




//DeliveryDBTest deliveryDBTest = new();

//await Task.Delay(1000);
//deliveryDBTest.SendPublicChat("啦啦啦啦啦123<0><W><0:17><0><W><0:18><0><W><0:19>");


//await Task.Delay(1000);
//MailItem mailItem = new()
//{
//    Id = 122649,
//    Count = 1,
//    ProcType = 19,
//    Guid1 = 1509020271,
//    Guid2 = 16779733,
//    Mask = 1073741825,
//    Data = "5f004000320000002c01000048710000487100002c000000010000000d0000000e00000062210000ab040000ab08000000000000000000001c0000000000a0410000a04002003f006031000060310000060000008d25000004000000a521000002000000c52500000e00000086a400006400000086a4000064000000d1460000b00400000c000000"
//};
//deliveryDBTest.SendMail(64, "Title", "Content", 2000000000, mailItem);







GameDBTest gameDB = new();

await Task.Delay(1000);
var roles = await gameDB.GetRolesAsync(64);
foreach (var userRole in roles)
{
    Console.WriteLine(userRole.Key + ":" + userRole.Value);
}



Console.ReadLine();