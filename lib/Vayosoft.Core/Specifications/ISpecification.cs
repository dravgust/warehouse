using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Models.Pagination;

namespace Vayosoft.Core.Specifications
{
    public interface ISpecification<T, TResult> : ISpecification<T>
    {
        Expression<Func<T, TResult>> Selector { get; } 
    }

    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Criteria { get; }
        ICollection<Expression<Func<T, object>>> Includes { get; }
        ICollection<string> IncludeStrings { get; }
    }

    public interface ISpecification2<T>
    {
        Expression<Func<T, bool>> Criteria { get; }
        ICollection<Expression<Func<T, bool>>> Includes { get; }
    }

    public interface IPagedSpecification<TEntity, TOrderKey> : ISpecification2<TEntity>, IPagingModel<TEntity, TOrderKey> where TEntity : class, IEntity
    {

    }
}
