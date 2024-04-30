namespace PwApi.Sockets;
internal static class Logger
{
    public static void Log(string msg)
    {
        Console.WriteLine($"{DateTime.Now:MM-dd HH:mm:ss.fff} : {msg}");
    }
}