namespace PW.Protocol.Models.DeliveryRecvs;

/// <summary>
/// 服务器主动推送的"服务器属性通告"包（包类型 0x84）。
/// DeliveryDB 在连接建立后下发当前服务器的运行属性，
/// 客户端可据此调整可用功能（如是否允许免费创角等）。
/// </summary>
public class AnnounceServerAttribute : IRecvPackage
{
    /// <summary>包类型常量，固定为 0x84。</summary>
    public uint Type => 0x84u;

    /// <summary>原始字节流，由底层收包逻辑回填。</summary>
    public byte[] Data { get; set; }

    /// <summary>
    /// 服务器属性位标志（4 字节整数）。
    /// 各 bit 含义由游戏侧定义，常用于开关试服、合服、内测等模式。
    /// </summary>
    public int Attr { get; private set; }

    /// <summary>
    /// 免费创建角色的截止时间（4 字节整数，通常为 Unix 时间戳）。
    /// 在该时间之前创建角色不收费。
    /// </summary>
    public int FreeCreatTime { get; private set; }

    /// <summary>
    /// 拆包：依次读取 4 字节属性位与 4 字节免费创角时间。
    /// </summary>
    /// <param name="up">已经按包头切分好的接收包数据。</param>
    public void UnPackFrom(RecvPackets up)
    {
        // 第一个 int：服务器属性位
        Attr = up.UnPackInt();
        // 第二个 int：免费创建角色的截止时间戳
        FreeCreatTime = up.UnPackInt();
    }

    /// <summary>
    /// 调试输出，包含包名与两个字段值。
    /// </summary>
    /// <returns>用于日志/控制台显示的字符串。</returns>
    public override string ToString()
    {
        return $"AnnounceServerAttribute---Attr={Attr},FreeCreatTime={FreeCreatTime}";
    }
}
