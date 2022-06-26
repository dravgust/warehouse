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
    public class PagedQuery<TEntity> : PagingModelBase<TEntity, object>, IQuery<IPagedEnumerable<TEntity>> where TEntity : class, IEntity
    {
        public PagedQuery(int page, int take, Sorting<TEntity, object> sorting, Filtering<TEntity> filtering)
            : base(page, take, sorting, filtering)
        {

        }

        public PagedQuery(int page, int take, Sorting<TEntity, object> sorting)
            : base(page, take, sorting)
        {

        }

        public PagedQuery()
        { }

        protected override Sorting<TEntity, object> BuildDefaultSorting()
            => new(x => x.Id, SortOrder.Desc);

    }

    public class PagedQueryHandler<TEntity> : IQueryHandler<PagedQuery<TEntity>, IPagedEnumerable<TEntity>>
        where TEntity : class, IEntity<string>
    {
        protected readonly IMongoCollection<TEntity> Collection;

        public PagedQueryHandler(IMongoContext context)
        {
            Collection = context.Database.GetCollection<TEntity>(CollectionName.For<TEntity>());
        }

        public Task<IPagedEnumerable<TEntity>> Handle(PagedQuery<TEntity> request, CancellationToken cancellationToken)
        {
            return Collection.AggregateByPage(request, cancellationToken);
        }
    }
}
