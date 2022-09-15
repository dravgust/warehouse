using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Vayosoft.Core.SharedKernel.Models.Pagination;

namespace Vayosoft.Core.Specifications
{
    public interface ISpecification<TEntity, TSortKey, TResult> : ISpecification<TEntity, TSortKey> where TEntity : class
    {
        Expression<Func<TEntity, TResult>> Selector { get; } 
    }

    public interface ISpecification<TEntity, TSortKey> : IPagingModel<TEntity, TSortKey> where TEntity : class
    {
        Expression<Func<TEntity, bool>> Criteria { get; }

        ICollection<Expression<Func<TEntity, bool>>> WhereExpressions { get; }

        ICollection<Expression<Func<TEntity, object>>> Includes { get; }

        ICollection<string> IncludeStrings { get; }
    }
}
