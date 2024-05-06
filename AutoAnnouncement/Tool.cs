using System;
using System.IO;
using System.Threading.Tasks;
using PW.Protocol.Comm;
using PW.Protocol.Models.DeliveryRecvs;
using PW.Protocol.Models.DeliverySends;

namespace AutoAnnouncement;
internal static class Tool
{
    public static BaseClient deliveryDB;

    public static void Connect()
    {
        Task.Run(async () =>
        {
            while (true)
            {
                if (deliveryDB == null || !deliveryDB.IsRunning)
                {
                    try
                    {
                        deliveryDB = new();
                        deliveryDB.Connect("192.168.2.178", 29100);
                        deliveryDB.AddReceive<ChatBroadCast>(x => LogChatBroadCast(x.ToString(), x.Data));
                        deliveryDB.AddReceive<WorldChat>(x => LogChatBroadCast(x.ToString(), x.Data));
                        Console.WriteLine($"{DateTime.Now:dd-HH:mm:ss}--服务器连接成功");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"{DateTime.Now:dd-HH:mm:ss}--服务器连接失败{e}");
                    }
                }
                await Task.Delay(3000);
            }
        });
    }

    public static void Send(string message)
    {
        try
        {
            deliveryDB.Send(new PublicChat { Message = message });

            Console.WriteLine($"{DateTime.Now:dd-HH:mm:ss}--已发送消息:{message}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"{DateTime.Now:dd-HH:mm:ss}--发送消息失败:{message}{Environment.NewLine}{e}");
        }

    }

    private static void LogChatBroadCast(string text, byte[] data)
    {
        string hex = $"{DateTime.Now:HH:mm:ss}:  {data.ToHexString()}{Environment.NewLine}";
        Console.Write(hex);
        File.AppendAllText(DateTime.Now.ToString("MM_dd") + ".log", hex);

        string log = $"{DateTime.Now:HH:mm:ss}:  {text}{Environment.NewLine}";
        Console.Write(log);
        File.AppendAllText(DateTime.Now.ToString("MM_dd") + ".log", log);


    }

}