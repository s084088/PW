// See https://aka.ms/new-console-template for more information
using System;
using PwApi;
using PwApi.Models;

Console.WriteLine("Hello, World!");


byte[] bytes = [0xFF, 0xFF, 0xFF, 0xFF];


//DeliveryDB deliveryDB = new("192.168.200.100", 29100);

GameDB gameDB = new("192.168.200.100", 29400);

GetUserRoles getUserRoles = new();
getUserRoles.Send.UserId = 64;

gameDB.Send(getUserRoles);

foreach (var userRole in getUserRoles.Recv.Roles)
{
    Console.WriteLine(userRole.Id + ":" + userRole.Name);
}


////公告
//deliveryDB.AddRecvPackageProcess<ChatBroadCast>(x => Console.WriteLine("12312312312312" + x.Message));
//await Task.Delay(1000);
//PublicChat publicChat = new() { Message = "啦啦啦啦啦123<0><W><0:17><0><W><0:18><0><W><0:19>" };
//deliveryDB.Send(publicChat);

////邮件
//await Task.Delay(1000);
//SysSendMail sysSendMail = new();
//sysSendMail.AttachMoney = 2000000000;
//sysSendMail.Title = "Title";
//sysSendMail.Context = "Contenx";
//sysSendMail.Receiver = 64;

//sysSendMail.AttachObj.Id = 12649;
//sysSendMail.AttachObj.Data = "5f004000320000002c01000048710000487100002c000000010000000d0000000e00000062210000ab040000ab08000000000000000000001c0000000000a0410000a04002003f006031000060310000060000008d25000004000000a521000002000000c52500000e00000086a400006400000086a4000064000000d1460000b00400000c000000";

//sysSendMail.AttachObj.Count = 1;
//sysSendMail.AttachObj.Guid1 = 1509020271;
//sysSendMail.AttachObj.Guid2 = 16779733;
//sysSendMail.AttachObj.Mask = 1073741825;
//sysSendMail.AttachObj.ProcType = 19;
//deliveryDB.Send(sysSendMail);




//Console.WriteLine("Finish");
Console.ReadLine();