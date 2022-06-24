using System;
using System.Linq.Expressions;

namespace Vayosoft.Core.SharedKernel.Models
{
    public class Filtering<TEntity> : Filtering<TEntity, object> where TEntity : class
    {
        public Filtering(Expression<Func<TEntity, object>> expression, string filter)
            : base(expression, filter)
        {
        }
    }

    public class Filtering<TEntity, TKey> where TEntity : class
    {
        public Filtering(Expression<Func<TEntity, TKey>> expression, string filter)
        {
            Filter = filter;
            Expression = expression;
        }

        public string Filter { get; }
        public Expression<Func<TEntity, TKey>> Expression { get; }
    }
}
