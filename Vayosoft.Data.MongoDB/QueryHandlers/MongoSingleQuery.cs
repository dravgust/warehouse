using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Vayosoft.Core.Persistence.Queries;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.Utilities;

namespace Vayosoft.Data.MongoDB.QueryHandlers
{
    public class MongoSingleQueryHandler<TKey, TEntity> : IQueryHandler<SingleQuery<TEntity>, TEntity>
        where TKey : IComparable, IComparable<TKey>, IEquatable<TKey>
        where TEntity : class, IEntity<TKey>
    {
        protected readonly IMongoCollection<TEntity> Collection;

        public MongoSingleQueryHandler(IMongoContext context)
        {
            Collection = context.Collection<TEntity>();
        }

        public virtual async Task<TEntity> Handle(SingleQuery<TEntity> requiest, CancellationToken cancellationToken)
        {
            Guard.NotNull(requiest.Id, nameof(requiest.Id));
            var result = await Collection.FindAsync(x => requiest.Id.Equals(x.Id), cancellationToken: cancellationToken);
            return result.SingleOrDefault(cancellationToken);
        }
    }
}
