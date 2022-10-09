using System.Reflection;
using AutoMapper;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Entities;

namespace Vayosoft.AutoMapper
{
    public class DtoEntityTypeConverter<TKey, TDto, TEntity> : ITypeConverter<TDto, TEntity>
            where TEntity : class, IEntity<TKey>, new()     
    {
        private readonly IUnitOfWork _unitOfWork;

        public DtoEntityTypeConverter(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public TEntity Convert(TDto source, TEntity destination, ResolutionContext context)
        {
            var sourceId = (source as IEntity)?.Id;

            var dest = destination ?? (sourceId != null
                ? _unitOfWork.Find<TEntity>(sourceId) ?? new TEntity()
                : new TEntity());

            // Да, reflection, да медленно и может привести к ошибкам в рантайме.
            // Можете написать Expression Trees, скомпилировать и закешировать для производительности
            // И анализатор для проверки корректности Dto на этапе компиляции
            var sp = typeof(TDto)
                .GetTypeInfo()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.CanRead && x.CanWrite)
                .ToDictionary(x => x.Name.ToUpper(), x => x);

            var dp = typeof(TEntity)
                .GetTypeInfo()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.CanRead && x.CanWrite)
                .ToArray();

            // проходимся по всем свойствам целевого объекта
            foreach (var propertyInfo in dp)
            {
                var key = typeof(IEntity).GetTypeInfo().IsAssignableFrom(propertyInfo.PropertyType)
                    ? propertyInfo.Name.ToUpper() + "ID"
                    : propertyInfo.Name.ToUpper();

                if (!sp.ContainsKey(key)) continue;

                // маппим один к одному примитивы, связанные сущности тащим из контекста
                if (key.EndsWith("ID")
                    && typeof(IEntity).GetTypeInfo().IsAssignableFrom(propertyInfo.PropertyType))
                {
                    var method = _unitOfWork.GetType().GetMethods()
                        .Where(x => x.Name == nameof(_unitOfWork.Find))
                        .First(x => x.IsGenericMethod);
                    var generic = method.MakeGenericMethod(propertyInfo.PropertyType);

                    propertyInfo.SetValue(dest, generic.Invoke(this, new[] { sp[key].GetValue(source) }));
                }
                else
                {
                    if (propertyInfo.PropertyType != sp[key].PropertyType)
                    {
                        throw new InvalidOperationException($"Can't map Property {propertyInfo.Name} because of type mismatch:" +
                                                            $"{sp[key].PropertyType.Name} -> {propertyInfo.PropertyType.Name}");
                    }

                    propertyInfo.SetValue(dest, sp[key].GetValue(source));
                }
            }

            return dest;
        }
    }
}
