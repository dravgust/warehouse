using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Vayosoft.Core.Persistence.Queries;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Specifications;

namespace Vayosoft.Data.MongoDB.QueryHandlers
{
    using static String;

    public class MongoPagingQueryHandler<TSpecification, TEntity> :
        IQueryHandler<SpecificationQuery<TSpecification, IPagedEnumerable<TEntity>>, IPagedEnumerable<TEntity>>
        where TEntity : class, IEntity
        where TSpecification : IPagingModel<TEntity, object>
    {
        protected readonly IMongoCollection<TEntity> Collection;

        public MongoPagingQueryHandler(IMongoDbContext context)
        {
            Collection = context.Collection<TEntity>();
        }

        public Task<IPagedEnumerable<TEntity>> Handle(SpecificationQuery<TSpecification, IPagedEnumerable<TEntity>> request, CancellationToken cancellationToken)
        {
            var spec = request.Specification;

            FilterDefinition<TEntity> filter = null;
            if (spec is IFilteringSpecification<TEntity> filterSpecification && !IsNullOrEmpty(filterSpecification.FilterString))
            {
                foreach (var field in filterSpecification.FilterBy)
                {
                    var pattern = new Regex(".*" + filterSpecification.FilterString + ".*", RegexOptions.IgnoreCase);
                    var regularExpression = BsonRegularExpression.Create(pattern);

                    if (filter == null)
                        filter = Builders<TEntity>.Filter.Regex(field, regularExpression);
                    else
                        filter |= Builders<TEntity>.Filter.Regex(field, regularExpression);
                }
            }

            filter ??= Builders<TEntity>.Filter.Empty;

            return Collection.AggregateByPage(spec, filter, cancellationToken);
        }
    }
}
