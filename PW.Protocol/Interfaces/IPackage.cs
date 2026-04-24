using PW.Protocol.Packages;

namespace PW.Protocol.Interfaces;

/// <summary>
/// 协议包的最小公共契约。
/// 任何在网络上传输的包都必须暴露唯一的 <see cref="Type"/> 标识，
/// 以便发送方与接收方按类型号路由处理。
/// </summary>
public interface IPackage
{
    /// <summary>协议包类型号；决定收发双方按何种结构解析包体。</summary>
    uint Type { get; }
}


/// <summary>
/// 发送包契约：兼具包标识 (<see cref="IPackage"/>) 与序列化能力 (<see cref="IPackTo"/>)。
/// 由 <c>BaseClient.Send</c> 把它写入底层 socket。
/// </summary>
public interface ISendPackage : IPackage, IPackTo
{

}

/// <summary>
/// 接收包契约：兼具包标识 (<see cref="IPackage"/>) 与反序列化能力 (<see cref="IUnPackFrom"/>)。
/// 由 <c>BaseClient.Analysis</c> 在拆出完整一帧后实例化、回填 <see cref="Data"/>、再调用 <c>UnPackFrom</c>。
/// </summary>
public interface IRecvPackage : IPackage, IUnPackFrom
{
    /// <summary>包体原始字节（不含外层包头），由收包流程在解包前注入。</summary>
    byte[] Data { get; internal set; }
}

/// <summary>
/// 同步请求-响应包契约：将一次「发出 <typeparamref name="TSend"/>，等待 <typeparamref name="TRecv"/>」
/// 的调用语义封装在一个包对象中，供 <c>BaseClient.Send&lt;TSend, TRecv&gt;</c> 配对使用。
/// </summary>
/// <typeparam name="TSend">请求体类型，必须能够 <see cref="IPackTo"/> 序列化。</typeparam>
/// <typeparam name="TRecv">响应体类型，必须能够 <see cref="IUnPackFrom"/> 反序列化。</typeparam>
public interface ICallPackage<TSend, TRecv> : IPackage where TSend : IPackTo where TRecv : IUnPackFrom
{
    /// <summary>请求体；调用方填充后由发送流程序列化送出。</summary>
    TSend Send { get; set; }

    /// <summary>响应体；收包流程反序列化后回填供调用方读取。</summary>
    TRecv Recv { get; set; }
}
