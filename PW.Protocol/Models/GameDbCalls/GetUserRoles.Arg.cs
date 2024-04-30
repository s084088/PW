namespace PW.Protocol.Models.GameDbCalls;

public record GetUserRolesArg : IPackTo
{
    public int UserId { get; set; }

    public void PackTo(SendPackets packets)
    {
        packets.Pack(UserId);
    }
}