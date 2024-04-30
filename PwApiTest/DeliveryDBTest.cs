using System;
using PW.Protocol.Comm;
using PW.Protocol.Models.DeliveryRecvs;
using PW.Protocol.Models.DeliverySends;

namespace PwApiTest;
internal class DeliveryDBTest
{
    private readonly BaseClient deliveryDB;

    public DeliveryDBTest()
    {
        deliveryDB = new();
        deliveryDB.Connect("192.168.200.100", 29100);

        //deliveryDB.AddSendedEvent(x => Console.WriteLine($"发送字节组---{x.Bytes.ToHexString()}"));    //所有发送
        //deliveryDB.AddReceivdEvent(x => Console.WriteLine($"接收字节组---{x.Bytes.ToHexString()}"));   //所有接收

        deliveryDB.AddRecvedPacketsEvent(x =>
        {

            if (x.RecvPackets.Type == 0)                //解包失败,打印字节组
                Console.WriteLine(x.RecvPackets.Data.ToHexString());

            else if (x.RecvPackage == null)             //没写对应的处理包
                Console.WriteLine(x.RecvPackets.Type);

            else if (x.RecvPackage is AnnounceServerAttribute attribute)  //这可能是心跳包,不处理
            {
                //Console.WriteLine("接收到attribute包");
            }
            else                                                          //打印其他收到的包
            {
                Console.WriteLine($"接收到数据包===={x.RecvPackage}");
            }
        });

        deliveryDB.AddReceive<ChatBroadCast>(x => Console.WriteLine("接收到公告---" + x.ToString()));

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

    public int ExpireDate { get; set; }

    public int Guid1 { get; set; }

    public int Guid2 { get; set; }

    public int Mask { get; set; }

    public string Data { get; set; } = string.Empty;
}