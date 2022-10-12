using System.Reflection;
using AutoMapper;
using Vayosoft.Core.Mapping;

namespace Vayosoft.AutoMapper
{
    public class ConventionalProfile : Profile
    {
        public static IDictionary<Type, Type[]> TypeMap;

        public static void Scan(params Assembly[] assemblies)
        {
            TypeMap = assemblies
                .SelectMany(x => x.GetTypes())
                .Where(x => x.GetTypeInfo().GetCustomAttribute<ConventionalMapAttribute>() != null)
                .GroupBy(x => x.GetTypeInfo().GetCustomAttribute<ConventionalMapAttribute>().EntityType)
                .ToDictionary(k => k.Key, v => v.ToArray());
        }

        public ConventionalProfile()
        {
            if (TypeMap == null)
            {
                throw new InvalidOperationException("Use ConventionalProfile.Scan method first!");
            }

            foreach (var kv in TypeMap)
            {
                foreach (var v in kv.Value)
                {
                    var attr = v.GetTypeInfo().GetCustomAttribute<ConventionalMapAttribute>();
                    if (attr.Direction is MapDirection.EntityToDto or MapDirection.Both)
                    {
                        CreateMap(kv.Key, v);
                    }
                    if (attr.Direction is MapDirection.DtoToEntity or MapDirection.Both)
                    {
                        CreateMap(v, kv.Key).ConvertUsing(typeof(DtoEntityTypeConverter<,,>)
                            .MakeGenericType(kv.Key.GetTypeInfo().GetProperty("Id")?.PropertyType, v, kv.Key));
                    }
                }
            }
        }
    }
}
