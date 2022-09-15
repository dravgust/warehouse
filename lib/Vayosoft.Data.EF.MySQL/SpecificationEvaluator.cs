using Microsoft.EntityFrameworkCore;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.Specifications;

namespace Vayosoft.Data.EF.MySQL
{
    public class SpecificationEvaluator<TEntity> : ISpecificationEvaluator<TEntity> where TEntity : class, IEntity
    {
        public IQueryable<TEntity> Evaluate(IQueryable<TEntity> input, ISpecification<TEntity> spec)
        {
            var query = input;
            if (spec.Criteria != null) query = query.Where(spec.Criteria);
            query = spec.WhereExpressions.Aggregate(query, (current, include) => current.Where(include));
            if (spec.OrderBy != null) query = query.OrderBy(spec.OrderBy);
            else if(spec.OrderByDescending != null) query = query.OrderByDescending(spec.OrderByDescending);
            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
            return query;
        }
    }
}
