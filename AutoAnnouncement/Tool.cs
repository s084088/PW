using System;
using System.IO;
using System.Threading.Tasks;
using PW.Protocol.Comm;
using PW.Protocol.Models.DeliveryRecvs;
using PW.Protocol.Models.DeliverySends;

namespace AutoAnnouncement;

/// <summary>
/// AutoAnnouncement 进程的全局工具类，封装与 DeliveryDB 服务器的长连接
/// 以及世界喊话的发送。
/// <para>
/// 与 PW.Protocol 协作流程：
/// <list type="number">
/// <item><description>持有共享的 <see cref="BaseClient"/> 实例（即 <see cref="deliveryDB"/>）。</description></item>
/// <item><description>通过 <see cref="BaseClient.AddReceive{T}(System.Action{T})"/> 订阅
/// <see cref="ChatBroadCast"/> / <see cref="WorldChat"/> 推送，把收到的喊话写入日志。</description></item>
/// <item><description>对外提供 <see cref="Send(string)"/>，把字符串包装为 <see cref="PublicChat"/> 包并发送。</description></item>
/// </list>
/// </para>
/// </summary>
internal static class Tool
{
    /// <summary>
    /// 与 DeliveryDB 建立的长连接客户端实例；由 <see cref="Connect"/> 创建并按需重连。
    /// </summary>
    public static BaseClient deliveryDB;

    /// <summary>
    /// 启动一个常驻后台任务，每 3 秒检查一次连接状态；当连接为空或已断开时
    /// 重新建立到 DeliveryDB 的 TCP 连接，并重新注册 <see cref="ChatBroadCast"/>、
    /// <see cref="WorldChat"/> 推送以及通用包接收事件。
    /// </summary>
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
                        // IP/端口硬编码，按部署环境调整
                        deliveryDB.Connect("192.168.2.178", 29100);
                        deliveryDB.AddReceive<ChatBroadCast>(x => LogChatBroadCast(x.ToString(), x.Data));
                        deliveryDB.AddReceive<WorldChat>(x => LogChatBroadCast(x.ToString(), x.Data));

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
                                //Console.WriteLine($"接收到数据包===={x.RecvPackage.GetType()}===={x.RecvPackage}");
                            }
                        });

                        Console.WriteLine($"{DateTime.Now:dd-HH:mm:ss}--服务器连接成功");
                    }
                    catch (Exception e)
                    {
                        // 失败不抛出，下一轮循环继续重试
                        Console.WriteLine($"{DateTime.Now:dd-HH:mm:ss}--服务器连接失败{e}");
                    }
                }
                await Task.Delay(3000);
            }
        });
    }

    /// <summary>
    /// 通过 <see cref="deliveryDB"/> 向 DeliveryDB 发送一条世界喊话。
    /// 内部封装为 <see cref="PublicChat"/> 包；发送结果（成功或异常）会输出到控制台。
    /// </summary>
    /// <param name="message">要发送的世界喊话文本内容。</param>
    public static void Send(string message)
    {
        try
        {
            deliveryDB.Send(new PublicChat { Message = message });

            Console.WriteLine($"{DateTime.Now:dd-HH:mm:ss}--已发送消息:{message}");
        }
        catch (Exception e)
        {
            // 连接未就绪或网络异常时记录日志但不让进程崩溃
            Console.WriteLine($"{DateTime.Now:dd-HH:mm:ss}--发送消息失败:{message}{Environment.NewLine}{e}");
        }

    }

    /// <summary>
    /// 把收到的聊天广播写入控制台并追加到按日期命名的日志文件（MM_dd.log）。
    /// </summary>
    /// <param name="text">已格式化的可读文本（通常来自包对象的 ToString()）。</param>
    /// <param name="data">原始字节数据，当前仅用于注释中的可选 hex 输出。</param>
    private static void LogChatBroadCast(string text, byte[] data)
    {
        //string hex = $"{DateTime.Now:HH:mm:ss}:  {data.ToHexString()}{Environment.NewLine}";
        //Console.Write(hex);
        //File.AppendAllText(DateTime.Now.ToString("MM_dd") + ".log", hex);

        string log = $"{DateTime.Now:HH:mm:ss}:  {text}{Environment.NewLine}";
        Console.Write(log);
        File.AppendAllText(DateTime.Now.ToString("MM_dd") + ".log", log);


    }

}
