using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Vayosoft.Core.SharedKernel.Models;

namespace Vayosoft.Core.Specifications
{
    public class PagedSpecification<TEntity> : SpecificationBase<TEntity>, IPagedSpecification<TEntity> where TEntity : class
    {
        public PagedSpecification(int page, int pageSize) : this(page, pageSize, null)
        { }

        public PagedSpecification(int page, int pageSize, Expression<Func<TEntity, bool>> criteria) : base(criteria)
        {
            Page = page;
            PageSize = pageSize;
        }

        public int Page { get; init; }
        public int PageSize { get; init; }
    }

    public class SpecificationBase<TEntity> : ISpecification<TEntity> where TEntity : class
    {
        public SpecificationBase()
        { }

        public SpecificationBase(Expression<Func<TEntity, bool>> criteria) {
            Criteria = criteria;
        }

        public Expression<Func<TEntity, bool>> Criteria { get; }

        public ICollection<Expression<Func<TEntity, bool>>> WhereExpressions { get; }
            = new List<Expression<Func<TEntity, bool>>>();

        public ICollection<Expression<Func<TEntity, object>>> Includes { get; }
            = new List<Expression<Func<TEntity, object>>>();

        public ICollection<string> IncludeStrings { get; }
            = new List<string>(10);

        public Sorting<TEntity> Sorting { get; private set; }

        protected void Where(Expression<Func<TEntity, bool>> whereExpression) {
            WhereExpressions.Add(whereExpression);
        }

        protected void WhereIf(bool condition, Expression<Func<TEntity, bool>> whereExpression) {
            if(condition) WhereExpressions.Add(whereExpression);
        }

        protected void Include(Expression<Func<TEntity, object>> includeExpression) {
            Includes.Add(includeExpression);
        }

        protected void OrderBy(Expression<Func<TEntity, object>> orderByExpression) {
            Sorting = new Sorting<TEntity>(orderByExpression, SortOrder.Asc);
        }

        protected void OrderByDescending(Expression<Func<TEntity, object>> orderByDescExpression) {
            Sorting = new Sorting<TEntity>(orderByDescExpression, SortOrder.Desc);
        }
    }
}
