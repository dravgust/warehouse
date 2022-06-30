using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Exceptions;

namespace Vayosoft.Core.Persistence
{
    public static class RepositoryExtensions
    {
        public static async Task<TEntity> GetAsync<TEntity, TKey>(this IRepositoryBase<TEntity, TKey> repository, TKey id, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            var entity = await repository.FindAsync(id, cancellationToken);

            return entity ?? throw EntityNotFoundException.For<TEntity>(id);
        }

        public static async Task<Unit> GetAndUpdateAsync<TEntity, TKey>(this IRepositoryBase<TEntity, TKey> repository, TKey id, Action<TEntity> action, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            var entity = await repository.GetAsync(id, cancellationToken);
            action(entity);
            await repository.UpdateAsync(entity, cancellationToken);

            return Unit.Value;
        }

        public static async Task<Unit> SetAsync<TDto, TEntity>(this IRepositoryBase<TEntity, string> repository, TDto dto, IMapper mapper, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity<string>
            where TDto : class, IEntity<string>
        {
            TEntity entity = null;
            if (!string.IsNullOrEmpty(dto.Id) && (entity = await repository.FindAsync(dto.Id, cancellationToken)) != null)
            {
                await repository.UpdateAsync(mapper.Map(dto, entity), cancellationToken);
            }
            else
            {
                await repository.AddAsync(mapper.Map<TEntity>(dto), cancellationToken);
            }

            return Unit.Value;
        }
    }
}
