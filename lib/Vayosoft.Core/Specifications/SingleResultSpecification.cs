using System;
using System.Linq.Expressions;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.Utilities;

namespace Vayosoft.Core.Specifications
{
    public class SingleResultSpecification<TEntity> : ISingleResultSpecification<TEntity> where TEntity : class, IEntity
    {
        public SingleResultSpecification() : this(criteria => true)
        { }

        public SingleResultSpecification(Expression<Func<TEntity, bool>> criteria)
        {
            Criteria = Guard.NotNull(criteria, nameof(criteria));
        }

        public Expression<Func<TEntity, bool>> Criteria { get; protected set; }
    }

    public class SingleResultSpecification<TEntity, TDto> : SingleResultSpecification<TEntity>, ISingleResultSpecification<TEntity, TDto> where TEntity : class, IEntity
    { }
}
