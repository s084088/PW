using System.Reflection;

namespace PwApi;

internal static class DeliveryRecvManage
{
    private static readonly Dictionary<uint, Type> packages = [];

    static DeliveryRecvManage()
    {
        IEnumerable<Type> packageTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(x => typeof(IDeliveryRecvPackage).IsAssignableFrom(x) && !x.IsAbstract);

        foreach (Type packageType in packageTypes)
        {
            PropertyInfo typeProperty = packageType.GetProperty(nameof(IDeliveryRecvPackage.Type));
            if (typeProperty != null && typeProperty.PropertyType == typeof(uint))
            {
                uint typeValue = (uint)typeProperty.GetValue(Activator.CreateInstance(packageType));
                packages[typeValue] = packageType;
            }
        }
    }

    public static IDeliveryRecvPackage GetPackage(uint type)
    {
        return packages.TryGetValue(type, out Type packageType) ? (IDeliveryRecvPackage)Activator.CreateInstance(packageType) : null;
    }
}