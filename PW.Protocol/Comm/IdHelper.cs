namespace PW.Protocol.Comm;
internal static class IdHelper
{
    private static int id = 0;

    internal static int GetId => id++;
}
