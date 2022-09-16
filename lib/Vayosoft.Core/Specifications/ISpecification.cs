using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Vayosoft.Core.SharedKernel.Models;

namespace Vayosoft.Core.Specifications
{
    public interface IPagedSpecification<TEntity> : ISpecification<TEntity> where TEntity : class
    {
        int Page { get; }
        int PageSize { get; }
    }

    public interface ISpecification<TEntity> where TEntity : class
    {
        Expression<Func<TEntity, bool>> Criteria { get; }
        ICollection<Expression<Func<TEntity, bool>>> WhereExpressions { get; }
        ICollection<Expression<Func<TEntity, object>>> Includes { get; }
        ICollection<string> IncludeStrings { get; }
        Sorting<TEntity> Sorting { get; }
    }

    public interface ISpecification<TEntity, TSortKey, TResult> : ISpecification<TEntity> where TEntity : class
    {
        //Expression<Func<TEntity, TResult>> Selector { get; } 
    }
}
