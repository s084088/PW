namespace PW.Protocol.Models.DeliveryRecvs;

public class AnnounceChallengeAlgo : IRecvPackage
{
    public uint Type => 0x88u;
    public byte[] Data { get; set; }

    public byte ChallengeAlgo { get; private set; }

    public void UnPackFrom(RecvPackets p)
    {
        ChallengeAlgo = p.UnPackByte();
    }

    public override string ToString()
    {
        return $"AnnounceChallengeAlgo---ChallengeAlgo={ChallengeAlgo}";
    }
}