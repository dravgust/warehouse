using System;
using System.Linq.Expressions;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Models;

namespace Vayosoft.Core.Specifications
{
    public class SpecificationBuilder<TEntity> where TEntity : class, IEntity
    {
        public Specification<TEntity> Specification { get; }

        protected SpecificationBuilder(Specification<TEntity> specification)
        {
            this.Specification = specification;
        }

        public static SpecificationBuilder<TEntity> Criteria(Expression<Func<TEntity, bool>> criteria)
        {
            return new SpecificationBuilder<TEntity>(new Specification<TEntity>(criteria));
        }

        public static Specification<TEntity> Empty() => Criteria(criteria => true).Build();
        public static Specification<TEntity> Query(Expression<Func<TEntity, bool>> criteria) => Criteria(criteria).Build();

        public SpecificationBuilder<TEntity> OrderBy(Expression<Func<TEntity, object>> expression)
        {
            Specification.OrderBy = new Sorting<TEntity>(expression);
            return this;
        }

        public SpecificationBuilder<TEntity> OrderByDescending(Expression<Func<TEntity, object>> expression)
        {
            Specification.OrderBy = new Sorting<TEntity>(expression, SortOrder.Desc);
            return this;
        }

        public SpecificationBuilder<TEntity> Page(int page)
        {
            Specification.Page = page;
            return this;
        }

        public SpecificationBuilder<TEntity> PageSize(int pageSize)
        {
            Specification.PageSize = pageSize;
            return this;
        }

        public SpecificationBuilder<TEntity> AddInclude(Expression<Func<TEntity, object>> includeExpression)
        {
            Specification.Includes.Add(includeExpression);
            return this;
        }

        public SpecificationBuilder<TEntity> AddInclude(string includeString)
        {
            Specification.IncludeStrings.Add(includeString);
            return this;
        }
        public SpecificationBuilder<TEntity> Where(Expression<Func<TEntity, bool>> includeExpression)
        {
            Specification.WhereExpressions.Add(includeExpression);
            return this;
        }

        public SpecificationBuilder<TEntity> WhereIf(bool condition, Expression<Func<TEntity, bool>> includeExpression)
        {
            if (condition)
            {
                Specification.WhereExpressions.Add(includeExpression);
            }
            return this;
        }

        public Specification<TEntity> Build()
        {
            return Specification;
        }
    }
}
