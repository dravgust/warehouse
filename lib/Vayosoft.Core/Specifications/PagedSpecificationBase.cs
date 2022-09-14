using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.SharedKernel.Models;

namespace Vayosoft.Core.Specifications
{
    public class PagedSpecificationBase<T> : PagingBase<T, object>, IPagedSpecification<T, object> where T : class, IEntity
    {
        public PagedSpecificationBase() : this(criteria => true)
        { }

        public PagedSpecificationBase(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        public Expression<Func<T, bool>> Criteria { get; }
        public ICollection<Expression<Func<T, object>>> Includes { get; }
            = new List<Expression<Func<T, object>>>();

        public ICollection<string> IncludeStrings { get; }
            = new List<string>();

        public ICollection<Expression<Func<T, bool>>> WhereExpressions { get; }
            = new List<Expression<Func<T, bool>>>();

        public PagedSpecificationBase<T> Where(Expression<Func<T, bool>> includeExpression)
        {
            WhereExpressions.Add(includeExpression);

            return this;
        }

        public PagedSpecificationBase<T> WhereIf(bool condition, Expression<Func<T, bool>> includeExpression)
        {
            if (condition)
            {
                WhereExpressions.Add(includeExpression);
            }

            return this;
        }

        protected override Sorting<T, object> BuildDefaultSorting() =>
            new(p => p.Id, SortOrder.Desc);
    }
}
