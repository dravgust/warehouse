using System.Linq;
using Vayosoft.Core.SharedKernel.Entities;

namespace Vayosoft.Core.Specifications
{
    public interface ISpecificationEvaluator<TEntity> where TEntity : class, IEntity
    {
        IQueryable<TEntity> Evaluate(IQueryable<TEntity> input, ISpecification<TEntity> spec);
    }
}
