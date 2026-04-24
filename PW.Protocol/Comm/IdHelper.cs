namespace PW.Protocol.Comm;

/// <summary>
/// 进程内自增 ID 生成器。
/// 用于给同步请求-响应包打上唯一序号，便于在 BaseClient 中进行匹配。
/// </summary>
internal static class IdHelper
{
    /// <summary>当前自增计数器，初始值为 0。</summary>
    private static int id = 0;

    /// <summary>
    /// 取下一个自增 ID（取值后再自增）。
    /// </summary>
    internal static int GetId => id++;
}
