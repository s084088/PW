# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 项目概述

PW 是一个基于原始 TCP socket 的"完美世界"系列网游服务器互动系统（C# / .NET 8.0）。它并不直接持久化数据，而是作为中间层连接到游戏方的 GameDB（角色/账号查询）和 DeliveryDB（世界喊话、邮件投递）两类服务器，对外提供定时公告、消息转发等能力。

## 常用命令

```bash
# 编译整个解决方案
dotnet build

# 运行定时公告服务（常驻进程，按日/周/月调度世界喊话）
dotnet run --project AutoAnnouncement/AutoAnnouncement.csproj

# 运行手工测试程序（非 xUnit/NUnit，纯 Console 应用；Program.cs 里手动注释/取消注释要跑的用例）
dotnet run --project PwApiTest/PwApiTest.csproj

# 只编译单个项目
dotnet build PwApi/PwApi.csproj
```

仓库**没有单元测试框架**、没有 CI（`.github/workflows/` 不存在）、没有 `.editorconfig`、没有 `dotnet format` 配置。"运行测试" = 运行 `PwApiTest` 并观察控制台输出与目标服务器的实际反应。

## 架构要点

### 解决方案与磁盘不一致（重要陷阱）

`Pw.slnx` 只列出了 4 个项目：`PW.Protocol`、`Util`、`AutoAnnouncement`、`PwApiTest`。但磁盘上还有两个未加入解决方案的项目为废弃项目。


用 Visual Studio 打开 `Pw.slnx` 看不到这两个项目；修改其中代码不会被 `dotnet build` 解决方案级构建覆盖。动手前先确认目标项目是否被解决方案实际引用。

### 包传输分层

从底到顶：

1. `Util/Libs/Sockets/SocketClient.cs` —— 裸 TCP 套接字封装（`Util` 项目）。
2. `PW.Protocol/Comm/BaseClient.cs` —— **核心路由层**，继承 `SocketClient`，同时负责收发包、按类型派发、以及同步请求-响应。
3. `PwApi/DeliveryDB.cs` / `PwApi/GameDB.cs` —— 业务门面，封装具体业务调用。
4. 可执行项目（`AutoAnnouncement`、`PwApiTest`）—— 调用门面。

### BaseClient 的三种收发模式

阅读 `PW.Protocol/Comm/BaseClient.cs` 才能理解业务代码里 `Send(...)` 的三种重载分别走哪条路径：

- `Send(ISendPackage)` —— 单向发出，不等回应。
- `Send<TSend, TRecv>(ICallPackage<...>)` —— **同步请求-响应**。通过 `ConcurrentDictionary<uint, RecvPackets> _calls` 按包 `Type` 占位，最多 `await Task.Delay(1)` 轮询 **1000 ms**；超时即静默返回 `null`，**不抛异常**。新加业务若服务器回包慢于 1 秒会"看起来没回应"，需要改此超时或改异步回调路径。
- `AddReceive<T>(Action<T>)` —— 事件订阅式。收到任意主动推送包后，`Analysis()` 通过 `RecvPackageRegister.GetPackage(type)` 反射得到包实例，再按类型从 `_recvActionList` 查到回调触发。

`Analysis()` 和 `ProcessPackge()` 中的 `catch { }` 目前是**完全吞掉异常**的空块（`BaseClient.cs` 约 124、144 行）。调试收包问题时，日志里看不到任何错误是正常现象——先在这两个 catch 里加临时日志。

### AutoAnnouncement 的调度方式

`AutoAnnouncement/Program.cs` 使用 **FluentScheduler 5.5.1** 的 `JobManager.AddJob(...)` 把一串固定的游戏活动提醒（蛇岛赛马、勇闯冥兽城、龙宫寻宝、夺宝骑兵、丛林遗迹、月度赛马等）挂在日/周/月 cron 风格表达式上，全部通过 `Tool.Send(text)` 转发到 DeliveryDB。**增加/修改公告就是在这个文件里加一行 `JobManager.AddJob`**——没有配置文件，全部硬编码。

## 目标服务器地址

硬编码在 `AutoAnnouncement` 与 `PwApiTest` 中，视环境可能需要调整：
