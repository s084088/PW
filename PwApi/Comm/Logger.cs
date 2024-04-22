namespace PwApi.Comm;

internal static class Logger
{
    public static void LogHex(string message)
    {
        Console.WriteLine($"ApiHex---{message}");
    }

    public static void LogPackets(string message)
    {
        Console.WriteLine($"ApiPackets---{message}");
    }


    public static void LogInfo(string message)
    {
        Console.WriteLine($"ApiInfo---{message}");
    }

    public static void LogError(string message)
    {
        Console.WriteLine($"ApiError---{message}");
    }
}