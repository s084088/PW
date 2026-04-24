using System;
using PW.Protocol.Comm;
using PW.Protocol.Models.DeliveryRecvs;
using PW.Protocol.Models.DeliverySends;

namespace PwApiTest;

/// <summary>
/// DeliveryDB（投递服务器）的手工测试用例集合。
/// 构造时自动连接到目标 DeliveryDB，
/// 并注册收包打印回调，方便在控制台直接观察服务器返回。
/// </summary>
internal class DeliveryDBTest
{
    /// <summary>底层投递服务器通讯客户端，承担收发包与事件分发。</summary>
    private readonly BaseClient deliveryDB;

    /// <summary>
    /// 构造时建立到 DeliveryDB 的 TCP 连接，并注册全部调试用收包回调：
    /// 解包失败 / 未知包类型 / 心跳包 / 其他业务包 分别走不同分支打印；
    /// 同时订阅 ChatBroadCast、WorldChat 主动推送以便观察聊天广播。
    /// </summary>
    public DeliveryDBTest()
    {
        deliveryDB = new();
        // 连接 DeliveryDB（地址 / 端口硬编码，需要根据实际环境调整）
        deliveryDB.Connect("192.168.200.100", 29100);

        //deliveryDB.AddSendedEvent(x => Console.WriteLine($"发送字节组---{x.Bytes.ToHexString()}"));    //所有发送
        //deliveryDB.AddReceivdEvent(x => Console.WriteLine($"接收字节组---{x.Bytes.ToHexString()}"));   //所有接收

        // 注册"已分析包"事件，分支处理各种到达情况
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
                Console.WriteLine($"接收到数据包===={x.RecvPackage.GetType()}===={x.RecvPackage}");
            }
        });



        // 订阅服务器主动推送的公告广播
        deliveryDB.AddReceive<ChatBroadCast>(x => Console.WriteLine("接收到公告---" + x.ToString()));
        // 订阅服务器主动推送的世界聊天
        deliveryDB.AddReceive<WorldChat>(x => Console.WriteLine("接收到世界聊天---" + x.ToString()));

    }

    /// <summary>
    /// 测试目标：向 DeliveryDB 发送一条 PublicChat（世界喊话）包。
    /// 依赖服务器：DeliveryDB。
    /// 预期表现：游戏内出现一条全服世界喊话，
    /// 同时本地控制台可能收到由服务器回推的 WorldChat 广播。
    /// </summary>
    /// <param name="message">要广播的喊话文本（可包含游戏内特殊标签）。</param>
    public void SendPublicChat(string message)
    {
        PublicChat publicChat = new();
        publicChat.Message = message;

        deliveryDB.Send(publicChat);
    }


    /// <summary>
    /// 测试目标：通过 DeliveryDB 给指定角色发送一封带附件（金钱 + 一件物品）的系统邮件。
    /// 依赖服务器：DeliveryDB。
    /// 预期表现：目标角色登录后能在游戏邮箱收到该系统邮件，附件含指定金额与物品。
    /// </summary>
    /// <param name="roleId">收件角色 RoleId。</param>
    /// <param name="title">邮件标题。</param>
    /// <param name="content">邮件正文。</param>
    /// <param name="money">附件金钱数量。</param>
    /// <param name="mailItem">附件物品；为 null 时使用空物品占位。</param>
    public void SendMail(int roleId, string title, string content, int money, MailItem mailItem)
    {
        SysSendMail sysSendMail = new();
        sysSendMail.Title = title;
        sysSendMail.Content = content;
        sysSendMail.Receiver = roleId;
        sysSendMail.AttachMoney = money;

        // 入参为 null 时构造空物品占位，避免后续字段访问 NRE
        mailItem ??= new();
        sysSendMail.AttachObj.Id = mailItem.Id;
        sysSendMail.AttachObj.Count = mailItem.Count;
        sysSendMail.AttachObj.ProcType = mailItem.ProcType;
        sysSendMail.AttachObj.ExpireDate = mailItem.ExpireDate;
        sysSendMail.AttachObj.Guid1 = mailItem.Guid1;
        sysSendMail.AttachObj.Guid2 = mailItem.Guid2;
        sysSendMail.AttachObj.Mask = mailItem.Mask;
        sysSendMail.AttachObj.Data= mailItem.Data;

        deliveryDB.Send(sysSendMail);

    }
}

/// <summary>
/// 测试用邮件附件物品参数对象。
/// 各字段对应游戏物品在邮件附件协议里的字段，仅用于构造测试用例的便利结构体。
/// </summary>
internal class MailItem
{
    /// <summary>物品模板 Id（道具 Id）。</summary>
    public int Id { get; set; }

    /// <summary>物品堆叠数量。</summary>
    public int Count { get; set; }

    /// <summary>处理类型 / 来源类型，由游戏端定义。</summary>
    public int ProcType { get; set; }

    /// <summary>过期时间（Unix 秒），0 表示不过期。</summary>
    public int ExpireDate { get; set; }

    /// <summary>物品唯一标识高位。</summary>
    public int Guid1 { get; set; }

    /// <summary>物品唯一标识低位。</summary>
    public int Guid2 { get; set; }

    /// <summary>物品标志位掩码（绑定 / 锁定等）。</summary>
    public int Mask { get; set; }

    /// <summary>物品自定义数据（hex 字符串），描述强化 / 镶嵌等扩展属性。</summary>
    public string Data { get; set; } = string.Empty;
}
