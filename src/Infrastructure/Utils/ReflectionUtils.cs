using System;
using System.Linq;
using System.Reflection;

namespace Infrastructure.Utils
{
    public static class ReflectionUtils
    {
        public static string GetAssemblyVersion<T>()
        {
            return GetAssemblyVersion(typeof(T));
        }

        public static string GetAssemblyVersion(Type type)
        {
            var containingAssembly = type.GetTypeInfo().Assembly;

            return containingAssembly
                .GetCustomAttributes<AssemblyInformationalVersionAttribute>()
                .FirstOrDefault()?
                .InformationalVersion;
        }
    }
}