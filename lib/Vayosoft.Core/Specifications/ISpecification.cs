using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Vayosoft.Core.SharedKernel.Models;

namespace Vayosoft.Core.Specifications
{
    public interface ISpecification<TEntity, TResult> : ISpecification<TEntity> where TEntity : class
    {
        Expression<Func<TEntity, TResult>> Selector { get; } 
    }

    public interface ISpecification<TEntity> where TEntity : class
    {
        public int? Page { get; }

        public int? PageSize { get; }

        Sorting<TEntity, object> OrderBy { get; }

        Expression<Func<TEntity, bool>> Criteria { get; }

        ICollection<Expression<Func<TEntity, bool>>> WhereExpressions { get; }

        ICollection<Expression<Func<TEntity, object>>> Includes { get; }

        ICollection<string> IncludeStrings { get; }
    }
}
