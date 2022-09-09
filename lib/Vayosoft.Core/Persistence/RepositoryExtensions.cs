using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Exceptions;

namespace Vayosoft.Core.Persistence
{
    public static class RepositoryExtensions
    {
        public static async Task<T> GetAsync<T, TId>(this IReadOnlyRepositoryBase<T> repository, TId id, CancellationToken cancellationToken = default)
            where T : class, IEntity
        {
            var entity = await repository.FindAsync(id, cancellationToken);

            return entity ?? throw EntityNotFoundException.For<T>(id);
        }

        public static async Task<Unit> GetAndUpdateAsync<T, TId>(this IRepositoryBase<T> repository, TId id, Action<T> action, CancellationToken cancellationToken = default)
            where T : class, IEntity
        {
            var entity = await repository.GetAsync(id, cancellationToken);
            action(entity);
            await repository.UpdateAsync(entity, cancellationToken);

            return Unit.Value;
        }
    }
}
