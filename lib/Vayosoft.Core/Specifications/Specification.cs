using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Utilities;

namespace Vayosoft.Core.Specifications
{
    public class Specification<TEntity, TResult> : Specification<TEntity>, ISpecification<TEntity, object, TResult>
        where TEntity : class, IEntity
    {
        
    }

    public class Specification<TEntity> : PagingModelBase<TEntity, object>, ISpecification<TEntity, object> where TEntity : class, IEntity
    {
        public Specification() : this(criteria => true, null)
        { }

        public Specification(Expression<Func<TEntity, bool>> criteria)
            : this(criteria, new Sorting<TEntity>(e => e.Id, SortOrder.Asc))
        { }

        public Specification(int page, int pageSize, Sorting<TEntity, object> orderBy)
            : this(e => true, orderBy)
        {
            Page = page;
            PageSize = pageSize;
        }

        public Specification(Expression<Func<TEntity, bool>> criteria, Sorting<TEntity, object> orderBy) : base(orderBy)
        {
            Criteria = Guard.NotNull(criteria, nameof(criteria));
        }

        public Expression<Func<TEntity, bool>> Criteria { get; protected set; }

        public ICollection<Expression<Func<TEntity, object>>> Includes { get; }
            = new List<Expression<Func<TEntity, object>>>();

        public ICollection<string> IncludeStrings { get; }
            = new List<string>();

        public ICollection<Expression<Func<TEntity, bool>>> WhereExpressions { get; }
            = new List<Expression<Func<TEntity, bool>>>();


        protected override Sorting<TEntity, object> BuildDefaultSorting()
        {
            return new Sorting<TEntity>(e => e.Id, SortOrder.Asc);
        }
    }
}
