namespace PW.Protocol.Models.DeliveryRecvs;

/// <summary>
/// 服务器主动推送的"挑战算法通告"包（包类型 0x88）。
/// DeliveryDB 在握手阶段告知客户端使用的握手挑战算法编号，
/// 后续登录鉴权流程会按此编号选择对应的加密/校验方式。
/// </summary>
public class AnnounceChallengeAlgo : IRecvPackage
{
    /// <summary>包类型常量，固定为 0x88。</summary>
    public uint Type => 0x88u;

    /// <summary>原始字节流，由底层收包逻辑回填。</summary>
    public byte[] Data { get; set; }

    /// <summary>
    /// 服务器要求使用的挑战算法编号（1 字节）。
    /// 不同数值对应不同的握手算法。
    /// </summary>
    public byte ChallengeAlgo { get; private set; }

    /// <summary>
    /// 拆包：从协议字节流中读取 1 字节挑战算法编号。
    /// </summary>
    /// <param name="p">已经按包头切分好的接收包数据。</param>
    public void UnPackFrom(RecvPackets p)
    {
        // 仅有一个字节字段，直接读取即可
        ChallengeAlgo = p.UnPackByte();
    }

    /// <summary>
    /// 调试输出，包含包名与挑战算法编号。
    /// </summary>
    /// <returns>用于日志/控制台显示的字符串。</returns>
    public override string ToString()
    {
        return $"AnnounceChallengeAlgo---ChallengeAlgo={ChallengeAlgo}";
    }
}
