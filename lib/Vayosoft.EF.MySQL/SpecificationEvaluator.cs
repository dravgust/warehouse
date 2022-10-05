using Microsoft.EntityFrameworkCore;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.Specifications;

namespace Vayosoft.Data.EF.MySQL
{
    public class SpecificationEvaluator<TEntity> : Core.Specifications.SpecificationEvaluator<TEntity> where TEntity : class, IEntity
    {
        public new IQueryable<TEntity> Evaluate(IQueryable<TEntity> input, ISpecification<TEntity> spec)
        {
            var query = base.Evaluate(input, spec);
            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
            query = spec.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));
            return query;
        }
    }
}
