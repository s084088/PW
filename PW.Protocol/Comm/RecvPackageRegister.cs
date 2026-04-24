using System.Reflection;

namespace PW.Protocol.Comm;

/// <summary>
/// 收包类型注册表。
/// 在静态构造时通过反射扫描当前程序集，把所有非抽象的 <see cref="IRecvPackage"/> 实现
/// 按其 <see cref="IPackage.Type"/> 值索引起来。
/// 运行期由 <c>BaseClient.Analysis()</c> 调用 <see cref="GetPackage(uint)"/>
/// 根据收到的包类型号实例化对应的包对象，再交给后续解包/订阅回调流程。
/// </summary>
internal static class RecvPackageRegister
{
    /// <summary>包类型号到 CLR 类型的映射表（uint Type =&gt; 实现类型）。</summary>
    private static readonly Dictionary<uint, Type> packages = [];

    /// <summary>
    /// 静态构造：反射当前程序集，登记所有非抽象的 <see cref="IRecvPackage"/> 实现。
    /// 通过临时实例化读取 <see cref="IPackage.Type"/> 属性值作为索引键。
    /// </summary>
    static RecvPackageRegister()
    {
        // 扫描程序集中所有可实例化的 IRecvPackage 类型
        IEnumerable<Type> packageTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(x => typeof(IRecvPackage).IsAssignableFrom(x) && !x.IsAbstract);

        foreach (Type packageType in packageTypes)
        {
            // 通过反射读取该类型的 Type 属性
            PropertyInfo typeProperty = packageType.GetProperty(nameof(IRecvPackage.Type));
            if (typeProperty != null && typeProperty.PropertyType == typeof(uint))
            {
                // 临时实例化以获取 Type 属性的实际取值
                uint typeValue = (uint)typeProperty.GetValue(Activator.CreateInstance(packageType));
                // 登记到映射表（同 Type 后注册者覆盖前者）
                packages[typeValue] = packageType;
            }
        }
    }

    /// <summary>
    /// 根据包类型号实例化对应的收包对象。
    /// 由 <c>BaseClient.Analysis()</c> 在解析到完整一帧后调用。
    /// </summary>
    /// <param name="type">协议包类型号。</param>
    /// <returns>对应类型的全新 <see cref="IRecvPackage"/> 实例；若未注册过此类型则返回 <c>null</c>。</returns>
    public static IRecvPackage GetPackage(uint type)
    {
        return packages.TryGetValue(type, out Type packageType) ? (IRecvPackage)Activator.CreateInstance(packageType) : null;
    }
}
