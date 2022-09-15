using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Vayosoft.Core.Specifications
{
    public class SpecificationBase<TEntity> : ISpecification<TEntity> where TEntity : class
    {
        public SpecificationBase()
        { }

        public SpecificationBase(Expression<Func<TEntity, bool>> criteria)
        {
            Criteria = criteria;
        }

        public Expression<Func<TEntity, bool>> Criteria { get; }

        public ICollection<Expression<Func<TEntity, bool>>> WhereExpressions { get; }
            = new List<Expression<Func<TEntity, bool>>>();

        public ICollection<Expression<Func<TEntity, object>>> Includes { get; }
            = new List<Expression<Func<TEntity, object>>>();

        public Expression<Func<TEntity, object>> OrderBy { get; private set; }

        public Expression<Func<TEntity, object>> OrderByDescending { get; private set; }

        protected void AddInclude(Expression<Func<TEntity, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        protected void AddOrderBy(Expression<Func<TEntity, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        protected void AddOrderByDescending(Expression<Func<TEntity, object>> orderByDescExpression)
        {
            OrderByDescending = orderByDescExpression;
        }
    }
}
