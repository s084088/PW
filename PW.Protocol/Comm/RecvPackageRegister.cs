using System.Reflection;

namespace PW.Protocol.Comm;

internal static class RecvPackageRegister
{
    private static readonly Dictionary<uint, Type> packages = [];

    static RecvPackageRegister()
    {
        IEnumerable<Type> packageTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(x => typeof(IRecvPackage).IsAssignableFrom(x) && !x.IsAbstract);

        foreach (Type packageType in packageTypes)
        {
            PropertyInfo typeProperty = packageType.GetProperty(nameof(IRecvPackage.Type));
            if (typeProperty != null && typeProperty.PropertyType == typeof(uint))
            {
                uint typeValue = (uint)typeProperty.GetValue(Activator.CreateInstance(packageType));
                packages[typeValue] = packageType;
            }
        }
    }

    public static IRecvPackage GetPackage(uint type)
    {
        return packages.TryGetValue(type, out Type packageType) ? (IRecvPackage)Activator.CreateInstance(packageType) : null;
    }
}