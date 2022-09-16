using System.Linq.Expressions;
using System;
using Vayosoft.Core.SharedKernel.Entities;

namespace Vayosoft.Core.Specifications
{
    public interface ICriteriaSpecification<TEntity, TDto> :
        ICriteriaSpecification<TEntity> where TEntity : class, IEntity
    { }

    public interface ICriteriaSpecification<TEntity> where TEntity : class, IEntity
    {
        Expression<Func<TEntity, bool>> Criteria { get; }
    }
}
