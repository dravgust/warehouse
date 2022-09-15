using System.Linq.Expressions;
using System;
using Vayosoft.Core.SharedKernel.Entities;

namespace Vayosoft.Core.Specifications
{
    public interface ISingleResultSpecification<TEntity, TDto> : ISingleResultSpecification<TEntity> where TEntity : class, IEntity
    { }

    public interface ISingleResultSpecification<TEntity> where TEntity : class, IEntity
    {
        Expression<Func<TEntity, bool>> Criteria { get; }
    }
}
