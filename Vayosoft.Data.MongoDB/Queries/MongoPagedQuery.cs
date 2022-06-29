using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.SharedKernel.Queries.Handler;
using Vayosoft.Core.SharedKernel.Queries.Query;

namespace Vayosoft.Data.MongoDB.Queries
{
    public class MongoPagedQuery<TEntity, TResult> : PagingModelBase<TEntity, object>, IQuery<TResult> where TEntity : class, IEntity
    {
        public MongoPagedQuery(int page, int take, Sorting<TEntity, object> sorting, Filtering<TEntity> filtering)
            : base(page, take, sorting, filtering)
        {

        }

        public MongoPagedQuery(int page, int take, Sorting<TEntity, object> sorting)
            : base(page, take, sorting)
        {

        }

        public MongoPagedQuery()
        { }

        protected override Sorting<TEntity, object> BuildDefaultSorting()
            => new(x => x.Id, SortOrder.Desc);

    }

    public class MongoPagedQueryHandler<TEntity> : IQueryHandler<MongoPagedQuery<TEntity, IPagedEnumerable<TEntity>>, IPagedEnumerable<TEntity>>
        where TEntity : class, IEntity<string>
    {
        protected readonly IMongoCollection<TEntity> Collection;

        public MongoPagedQueryHandler(IMongoContext context)
        {
            Collection = context.Database.GetCollection<TEntity>(CollectionName.For<TEntity>());
        }

        public Task<IPagedEnumerable<TEntity>> Handle(MongoPagedQuery<TEntity, IPagedEnumerable<TEntity>> request, CancellationToken cancellationToken)
        {
            return Collection.AggregateByPage(request, cancellationToken);
        }
    }
}
