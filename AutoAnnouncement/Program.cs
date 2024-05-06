// See https://aka.ms/new-console-template for more information

using System;
using System.IO;
using FluentScheduler;
using PW.Protocol.Comm;
using PW.Protocol.Models.DeliveryRecvs;
using PW.Protocol.Models.DeliverySends;

Console.WriteLine("Started");

BaseClient deliveryDB = new();
deliveryDB.Connect("192.168.2.178", 29100);

deliveryDB.AddReceive<ChatBroadCast>(x => LogChatBroadCast(x.ToString()));
deliveryDB.AddReceive<WorldChat>(x => LogChatBroadCast(x.ToString()));

JobManager.AddJob(() => SendMessage("《蛇岛赛马》活动将于15分钟后开始，可从各主城小狼处入场"), s => s.ToRunEvery(1).Days().At(12, 15));
JobManager.AddJob(() => SendMessage("《蛇岛赛马》活动将于5分钟后开始，可从各主城小狼处入场"), s => s.ToRunEvery(1).Days().At(12, 25));
JobManager.AddJob(() => SendMessage("《蛇岛赛马》活动将于15分钟后开始，可从各主城小狼处入场"), s => s.ToRunEvery(1).Days().At(21, 15));
JobManager.AddJob(() => SendMessage("《蛇岛赛马》活动将于5分钟后开始，可从各主城小狼处入场"), s => s.ToRunEvery(1).Days().At(21, 25));

JobManager.AddJob(() => SendMessage("《勇闯冥兽城》活动将于15分钟后开始，可从各主城小狼处入场"), s => s.ToRunEvery(1).Weeks().On(DayOfWeek.Monday).At(19, 45));
JobManager.AddJob(() => SendMessage("《勇闯冥兽城》活动将于5分钟后开始，可从各主城小狼处入场"), s => s.ToRunEvery(1).Weeks().On(DayOfWeek.Monday).At(19, 55));

JobManager.AddJob(() => SendMessage("《龙宫寻宝》活动将于15分钟后开始，可从各主城小狼处入场"), s => s.ToRunEvery(1).Weeks().On(DayOfWeek.Tuesday).At(19, 45));
JobManager.AddJob(() => SendMessage("《龙宫寻宝》活动将于5分钟后开始，可从各主城小狼处入场"), s => s.ToRunEvery(1).Weeks().On(DayOfWeek.Tuesday).At(19, 55));

JobManager.AddJob(() => SendMessage("《夺宝骑兵》活动将于15分钟后开始，可从祖龙城竞技场管理员处入场"), s => s.ToRunEvery(1).Weeks().On(DayOfWeek.Friday).At(17, 45));
JobManager.AddJob(() => SendMessage("《夺宝骑兵》活动将于5分钟后开始，可从祖龙城竞技场管理员处入场"), s => s.ToRunEvery(1).Weeks().On(DayOfWeek.Friday).At(17, 55));

JobManager.AddJob(() => SendMessage("《丛林遗迹》活动将于15分钟后开始，可从各主城小狼处入场"), s => s.ToRunEvery(1).Weeks().On(DayOfWeek.Sunday).At(14, 45));
JobManager.AddJob(() => SendMessage("《丛林遗迹》活动将于15分钟后开始，可从各主城小狼处入场"), s => s.ToRunEvery(1).Weeks().On(DayOfWeek.Sunday).At(14, 55));

JobManager.AddJob(() => SendMessage("《月度赛马》活动将于1小时后开始，即将停止报名！"), s => s.ToRunEvery(1).Months().On(28).At(18, 10));
JobManager.AddJob(() => SendMessage("《月度赛马》活动将于20分钟后开始，可从各主城赛马点准备"), s => s.ToRunEvery(1).Months().On(28).At(18, 50));
JobManager.AddJob(() => SendMessage("《月度赛马》活动将于5分钟后开始，可从各主城赛马点准备"), s => s.ToRunEvery(1).Months().On(28).At(19, 05));

JobManager.Start();




void SendMessage(string text) => deliveryDB.Send(new PublicChat { Message = text });

static void LogChatBroadCast(string text)
{
    string log = $"{DateTime.Now:HH:mm:ss}:  {text}{Environment.NewLine}";

    Console.Write(log);
    File.AppendAllText(DateTime.Now.ToString("MM_dd") + ".log", log);
}
