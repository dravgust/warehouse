using System.Collections.Concurrent;
using System.Reflection;

namespace Vayosoft.Core.Utilities
{
    public static class TypeProvider
    {
        private static readonly ConcurrentDictionary<string, Type> TypeKeys = new();

        public static Type GetTypeFromAnyReferencingAssembly(string typeName)
        {
            return TypeKeys.GetOrAdd(typeName, name =>
            {
                var referencedAssemblies = Assembly.GetEntryAssembly()?
                    .GetReferencedAssemblies()
                    .Select(a => a.FullName);

                if (referencedAssemblies == null)
                    return null;

                return AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => referencedAssemblies.Contains(a.FullName))
                    .SelectMany(a => a.GetTypes().Where(x => x.Name == name))
                    .FirstOrDefault();
            });
        }
    }
}
