using PW.Protocol.Packages;

namespace PW.Protocol.Interfaces;

/// <summary>
/// 表示一个可写入到 <see cref="SendPackets"/> 的可序列化对象。
/// 由发送类协议包实现，将自身字段按协议格式写入待发送字节缓冲。
/// </summary>
public interface IPackTo
{
    /// <summary>
    /// 把当前对象按协议字段顺序写入发送缓冲。
    /// </summary>
    /// <param name="p">用于追加字段的发送包缓冲。</param>
    void PackTo(SendPackets p);
}

/// <summary>
/// 表示一个可从 <see cref="RecvPackets"/> 反序列化的对象。
/// 由接收类协议包实现，从已收到的字节缓冲按协议字段顺序读出自身字段。
/// </summary>
public interface IUnPackFrom
{
    /// <summary>
    /// 从接收缓冲解析并填充当前对象的字段。
    /// </summary>
    /// <param name="p">已收齐字节、待解析的接收包缓冲。</param>
    void UnPackFrom(RecvPackets p);
}
