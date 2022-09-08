using System;
using System.Linq.Expressions;

namespace Vayosoft.Core.SharedKernel.Models
{
    public enum SortOrder
    {
        Asc = 1,
        Desc = 2
    }

    public class Sorting<TEntity> : Sorting<TEntity, object> where TEntity : class
    {
        public Sorting(Expression<Func<TEntity, object>> expression, SortOrder sortOrder = SortOrder.Asc) 
            : base(expression, sortOrder)
        {
        }
    }

    public class Sorting<TEntity, TKey>
        where TEntity : class
    {
        public Expression<Func<TEntity, TKey>> Expression { get; }

        public SortOrder SortOrder { get; }

        public Sorting(
            Expression<Func<TEntity, TKey>> expression,
            SortOrder sortOrder = SortOrder.Asc)
        {
            Expression = expression;
            SortOrder = sortOrder;
        }
    }
}