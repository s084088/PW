// See https://aka.ms/new-console-template for more information
using PwApi.Models;
using PwApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using AutoAnnouncement;

Console.WriteLine("Started");


DeliveryDB deliveryDB = new("192.168.2.178", 29100);

deliveryDB.AddRecvPackageProcess<ChatBroadCast>(x => LogChatBroadCast(x.ToString()));
deliveryDB.AddRecvPackageProcess<WorldChat>(x => LogChatBroadCast(x.ToString()));

List<Chat> chats = File.ReadAllLines("messages.txt")
    .Select(Chat.GetChat)
    .Where(x => x != null)
    .OrderBy(x => x.Time)
    .ToList();

if (chats.Count == 0)
{
    Console.WriteLine("没有读取到数据");
    Console.ReadLine();
    return;
}
while (chats[0].Time < DateTime.Now)
{
    Chat c = chats[0];
    chats.Remove(c);
    chats.Add(c.GetNextTime());
    chats = chats.OrderBy(x => x.Time).ToList();
}

Console.WriteLine($"载入了{chats.Count}条数据");


while (chats.Count > 0)
{
    Chat c = chats.FirstOrDefault();
    if (c.Time < DateTime.Now)
    {
        chats.Remove(c);
        chats.Add(c.GetNextTime());
        chats = chats.OrderBy(x => x.Time).ToList();

        PublicChat publicChat = new();
        publicChat.Message.AddString(c.Message);
        deliveryDB.Send(publicChat);
    }
    else
    {
        Thread.Sleep(1000);
    }
}




static void LogChatBroadCast(string text)
{
    string log = $"{DateTime.Now:HH:mm:ss}:  {text}{Environment.NewLine}";

    Console.Write(log);
    File.AppendAllText(DateTime.Now.ToString("MM_dd") + ".log", log);
}
