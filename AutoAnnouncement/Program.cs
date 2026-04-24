// AutoAnnouncement 进程入口（顶层语句）。
//
// 本进程是常驻定时公告调度器：使用 FluentScheduler 5.5.1 按日/周/月
// 调度完美世界游戏内的世界喊话提醒（蛇岛赛马、勇闯冥兽城、龙宫寻宝、
// 夺宝骑兵、丛林遗迹、月度赛马等活动）。
//
// 所有公告时间表均硬编码于本文件，没有任何配置文件；
// 新增或修改公告 = 在本文件追加/调整一行 JobManager.AddJob(...)。
// 实际世界喊话通过 Tool.Send(text) 转发到 DeliveryDB（PublicChat 包）。

using System;
using AutoAnnouncement;
using FluentScheduler;

Console.WriteLine("Started");


// 《蛇岛赛马》每天 12:30 开场，12:15 提前 15 分钟提醒
JobManager.AddJob(() => SendMessage("《蛇岛赛马》活动将于15分钟后开始，可从各主城小狼处入场"), s => s.ToRunEvery(1).Days().At(12, 15));
// 《蛇岛赛马》每天 12:30 开场，12:25 提前 5 分钟提醒
JobManager.AddJob(() => SendMessage("《蛇岛赛马》活动将于5分钟后开始，可从各主城小狼处入场"), s => s.ToRunEvery(1).Days().At(12, 25));
// 《蛇岛赛马》每天 21:30 开场，21:15 提前 15 分钟提醒
JobManager.AddJob(() => SendMessage("《蛇岛赛马》活动将于15分钟后开始，可从各主城小狼处入场"), s => s.ToRunEvery(1).Days().At(21, 15));
// 《蛇岛赛马》每天 21:30 开场，21:25 提前 5 分钟提醒
JobManager.AddJob(() => SendMessage("《蛇岛赛马》活动将于5分钟后开始，可从各主城小狼处入场"), s => s.ToRunEvery(1).Days().At(21, 25));

// 《勇闯冥兽城》每周一 20:00 开场，19:45 提前 15 分钟提醒
JobManager.AddJob(() => SendMessage("《勇闯冥兽城》活动将于15分钟后开始，可从各主城小狼处入场"), s => s.ToRunEvery(0).Weeks().On(DayOfWeek.Monday).At(19, 45));
// 《勇闯冥兽城》每周一 20:00 开场，19:55 提前 5 分钟提醒
JobManager.AddJob(() => SendMessage("《勇闯冥兽城》活动将于5分钟后开始，可从各主城小狼处入场"), s => s.ToRunEvery(0).Weeks().On(DayOfWeek.Monday).At(19, 55));

// 《龙宫寻宝》每周二 20:00 开场，19:45 提前 15 分钟提醒
JobManager.AddJob(() => SendMessage("《龙宫寻宝》活动将于15分钟后开始，可从各主城小狼处入场"), s => s.ToRunEvery(0).Weeks().On(DayOfWeek.Tuesday).At(19, 45));
// 《龙宫寻宝》每周二 20:00 开场，19:55 提前 5 分钟提醒
JobManager.AddJob(() => SendMessage("《龙宫寻宝》活动将于5分钟后开始，可从各主城小狼处入场"), s => s.ToRunEvery(0).Weeks().On(DayOfWeek.Tuesday).At(19, 55));

// 《夺宝骑兵》每周五 18:00 开场，17:45 提前 15 分钟提醒（祖龙城竞技场管理员处入场）
JobManager.AddJob(() => SendMessage("《夺宝骑兵》活动将于15分钟后开始，可从祖龙城竞技场管理员处入场"), s => s.ToRunEvery(0).Weeks().On(DayOfWeek.Friday).At(17, 45));
// 《夺宝骑兵》每周五 18:00 开场，17:55 提前 5 分钟提醒
JobManager.AddJob(() => SendMessage("《夺宝骑兵》活动将于5分钟后开始，可从祖龙城竞技场管理员处入场"), s => s.ToRunEvery(0).Weeks().On(DayOfWeek.Friday).At(17, 55));

// 《丛林遗迹》每周日 15:00 开场，14:45 提前 15 分钟提醒
JobManager.AddJob(() => SendMessage("《丛林遗迹》活动将于15分钟后开始，可从各主城小狼处入场"), s => s.ToRunEvery(0).Weeks().On(DayOfWeek.Sunday).At(14, 45));
// 《丛林遗迹》每周日 15:00 开场，14:55 二次提醒（文案故意保持与上一行一致）
JobManager.AddJob(() => SendMessage("《丛林遗迹》活动将于15分钟后开始，可从各主城小狼处入场"), s => s.ToRunEvery(0).Weeks().On(DayOfWeek.Sunday).At(14, 55));

// 《月度赛马》每月 28 日 19:10 开场，18:10 提前 1 小时提醒（即将停止报名）
JobManager.AddJob(() => SendMessage("《月度赛马》活动将于1小时后开始，即将停止报名！"), s => s.ToRunEvery(0).Months().On(28).At(18, 10));
// 《月度赛马》每月 28 日 19:10 开场，18:50 提前 20 分钟提醒
JobManager.AddJob(() => SendMessage("《月度赛马》活动将于20分钟后开始，可从各主城赛马点准备"), s => s.ToRunEvery(0).Months().On(28).At(18, 50));
// 《月度赛马》每月 28 日 19:10 开场，19:05 提前 5 分钟提醒
JobManager.AddJob(() => SendMessage("《月度赛马》活动将于5分钟后开始，可从各主城赛马点准备"), s => s.ToRunEvery(0).Months().On(28).At(19, 05));

JobManager.Start();

Tool.Connect();

/// <summary>
/// 调度任务的统一回调入口：把活动提醒文本通过 <see cref="Tool.Send(string)"/>
/// 转发到 DeliveryDB，由其下发为游戏内的世界喊话。
/// </summary>
/// <param name="text">完整的世界喊话文本，包含活动名与时间提示。</param>
void SendMessage(string text) => Tool.Send(text);

// 阻塞主线程防止进程退出
Console.ReadLine();
