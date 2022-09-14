using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.Utilities;

namespace Vayosoft.Core.Specifications
{
    public class SpecificationBase<TEntity> : ISpecification<TEntity> where TEntity : class, IEntity
    {
        private int? _page;

        private int? _size;

        public SpecificationBase(Expression<Func<TEntity, bool>> criteria)
            : this(new Sorting<TEntity, object>(e => e.Id, SortOrder.Asc), criteria)
        { }

        public SpecificationBase(int page, int pageSize, Sorting<TEntity, object> orderBy) 
            : this(orderBy, e => true)
        {
            Page = page;
            PageSize = pageSize;
        }

        public SpecificationBase(Sorting<TEntity, object> orderBy, Expression<Func<TEntity, bool>> criteria)
        {
            OrderBy = Guard.NotNull(orderBy, nameof(orderBy));
            Criteria = Guard.NotNull(criteria, nameof(criteria));
        }

        public int? Page
        {
            get => _page;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("page must be >= 0", nameof(value));
                }

                _page = value;
            }
        }

        public int? PageSize
        {
            get => _size;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("take must be > 0", nameof(value));
                }

                _size = value;
            }
        }

        public Sorting<TEntity, object> OrderBy { get; }

        public Expression<Func<TEntity, bool>> Criteria { get; }

        public ICollection<Expression<Func<TEntity, object>>> Includes { get; }
            = new List<Expression<Func<TEntity, object>>>();

        public ICollection<string> IncludeStrings { get; }
            = new List<string>();

        public ICollection<Expression<Func<TEntity, bool>>> WhereExpressions { get; }
            = new List<Expression<Func<TEntity, bool>>>();

        public SpecificationBase<TEntity> AddInclude(Expression<Func<TEntity, object>> includeExpression)
        {
            Includes.Add(includeExpression);

            return this;
        }

        public SpecificationBase<TEntity> AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);

            return this;
        }
        public SpecificationBase<TEntity> Where(Expression<Func<TEntity, bool>> includeExpression)
        {
            WhereExpressions.Add(includeExpression);

            return this;
        }

        public SpecificationBase<TEntity> WhereIf(bool condition, Expression<Func<TEntity, bool>> includeExpression)
        {
            if (condition)
            {
                WhereExpressions.Add(includeExpression);
            }

            return this;
        }
    }
}
