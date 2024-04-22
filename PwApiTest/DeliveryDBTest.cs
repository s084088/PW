using System;
using PwApi;
using PwApi.Models;

namespace PwApiTest;
internal class DeliveryDBTest
{
    private readonly DeliveryDB deliveryDB;

    public DeliveryDBTest()
    {
        deliveryDB = new("192.168.200.100", 29100);

        deliveryDB.AddRecvPackageProcess<ChatBroadCast>(x => Console.WriteLine("接收到事件ChatBroadCast---" + x.Message));

    }

    public void SendPublicChat(string message)
    {
        PublicChat publicChat = new();
        publicChat.Message.AddString(message);

        deliveryDB.Send(publicChat);
    }


    public void SendMail(int roleId, string title, string content, int money, MailItem mailItem)
    {
        SysSendMail sysSendMail = new();
        sysSendMail.Title.AddString(title);
        sysSendMail.Content.AddString(content);
        sysSendMail.Receiver = roleId;
        sysSendMail.AttachMoney = money;

        mailItem ??= new();
        sysSendMail.AttachObj.Id = mailItem.Id;
        sysSendMail.AttachObj.Count = mailItem.Count;
        sysSendMail.AttachObj.ProcType = mailItem.ProcType;
        sysSendMail.AttachObj.ExpireDate = mailItem.ExpireDate;
        sysSendMail.AttachObj.Guid1 = mailItem.Guid1;
        sysSendMail.AttachObj.Guid2 = mailItem.Guid2;
        sysSendMail.AttachObj.Mask = mailItem.Mask;
        sysSendMail.AttachObj.Data.AddHexString(mailItem.Data);

        deliveryDB.Send(sysSendMail);

    }
}

internal class MailItem
{
    public int Id { get; set; }

    public int Count { get; set; }

    public int ProcType { get; set; }

    public int ExpireDate {  get; set; }

    public int Guid1 { get; set; }

    public int Guid2 { get; set; }

    public int Mask { get; set; }

    public string Data { get; set; } = string.Empty;
}