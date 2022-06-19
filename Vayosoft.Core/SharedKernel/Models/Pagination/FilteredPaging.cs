using System;
using System.Linq.Expressions;
using Vayosoft.Core.SharedKernel.Entities;

namespace Vayosoft.Core.SharedKernel.Models.Pagination
{
    public class FilteredPaging<TEntity>: Paging<TEntity, object> 
        where TEntity : class, IEntity<string>
    {
        public readonly string FilterPattern;
        public readonly Expression<Func<TEntity, object>> FilterBy;

        public FilteredPaging(int page, int take, string filterPattern, Expression<Func<TEntity, object>> filterBy, Expression<Func<TEntity, object>> orderBy, SortOrder sortOrder)
            : base(page, take, new Sorting<TEntity, object>(orderBy, sortOrder))
        {
            FilterPattern = filterPattern;
            FilterBy = filterBy;
        }

        public FilteredPaging()
        { }

        protected override Sorting<TEntity, object> BuildDefaultSorting()
        {
            return new Sorting<TEntity, object>(x => x.Id, SortOrder.Desc);
        }
    }
}
