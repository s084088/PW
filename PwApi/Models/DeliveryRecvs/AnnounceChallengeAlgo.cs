namespace PwApi.Models;

public class AnnounceChallengeAlgo : IDeliveryRecvPackage
{
    public uint Type => 0x88u;

    public byte ChallengeAlgo { get; private set; }

    public void UnPack(UnPackets up)
    {
        ChallengeAlgo = up.UnPackByte();
    }
    public override string ToString()
    {
        return $"ChallengeAlgo={ChallengeAlgo}";
    }
}