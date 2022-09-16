using System.Linq;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Models;

namespace Vayosoft.Core.Specifications
{
    public class SpecificationEvaluator<TEntity> : ISpecificationEvaluator<TEntity> where TEntity : class, IEntity
    {
        public IQueryable<TEntity> Evaluate(IQueryable<TEntity> input, ISpecification<TEntity> spec)
        {
            var query = input;
            if (spec.Criteria != null) query = query.Where(spec.Criteria);
            query = spec.WhereExpressions.Aggregate(query, (current, include) => current.Where(include));
            if (spec.Sorting != null)
            {
                query = spec.Sorting.SortOrder == SortOrder.Asc
                    ? query.OrderBy(spec.Sorting.Expression)
                    : query.OrderByDescending(spec.Sorting.Expression);
            }

            return query;
        }
    }
}
