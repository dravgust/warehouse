using System.Collections.Concurrent;
using System.Reflection;

namespace Vayosoft.Core.Utilities
{
    public static class TypeExtensions
    {
        private static readonly ConcurrentDictionary<Type, string> TypeCacheKeys = new();
        public static string GetCacheKey(this Type type) => TypeCacheKeys.GetOrAdd(type, t => t.PrettyPrint());

        private static readonly ConcurrentDictionary<Type, string> PrettyPrintCache = new();
        public static string PrettyPrint(this Type type)
        {
            return PrettyPrintCache.GetOrAdd(type, t =>
                {
                    try
                    {
                        return PrettyPrintRecursive(t, 0);
                    }
                    catch (Exception)
                    {
                        return t.Name;
                    }
                });
        }

        private static string PrettyPrintRecursive(Type type, int depth)
        {
            if (depth > 3)
            {
                return type.Name;
            }

            var nameParts = type.Name.Split('`');
            if (nameParts.Length == 1)
            {
                return nameParts[0];
            }

            var genericArguments = type.GetTypeInfo().GetGenericArguments();
            return !type.IsConstructedGenericType
                ? $"{nameParts[0]}<{new string(',', genericArguments.Length - 1)}>"
                : $"{nameParts[0]}<{string.Join(",", genericArguments.Select(t => PrettyPrintRecursive(t, depth + 1)))}>";
        }

        public static TValue GetAttributeValue<TAttribute, TValue>(this Type type, Func<TAttribute, TValue> valueSelector) where TAttribute : Attribute
        {
            var attr = type.GetCustomAttributes<TAttribute>(true).FirstOrDefault();
            if (attr != null)
            {
                return valueSelector(attr);
            }
            return default;
        }

        public static string GetGenericTypeName(this Type type)
        {
            string typeName;
            if (type.IsGenericType)
            {
                var genericTypes = string.Join(",", type.GetGenericArguments().Select(t => t.Name).ToArray());
                typeName = $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
            }
            else
            {
                typeName = type.Name;
            }

            return typeName;
        }

        public static string GetGenericTypeName(this object @object) => @object.GetType().GetGenericTypeName();
    }
}
