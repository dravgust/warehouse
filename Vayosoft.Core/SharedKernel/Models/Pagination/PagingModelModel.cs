using Vayosoft.Core.SharedKernel.Entities;

namespace Vayosoft.Core.SharedKernel.Models.Pagination
{
    public class PagingModelModel<TEntity> : PagingModelModelBase<TEntity, object> 
        where TEntity : class, IEntity<string>
    {

        public PagingModelModel(int page, int take, Sorting<TEntity, object> sorting, Filtering<TEntity> filtering)
            : base(page, take, sorting, filtering)
        {
            
        }

        public PagingModelModel(int page, int take, Sorting<TEntity, object> sorting)
            : base(page, take, sorting)
        {

        }

        public PagingModelModel()
        { }

        protected override Sorting<TEntity, object> BuildDefaultSorting()
        {
            return new Sorting<TEntity, object>(x => x.Id, SortOrder.Desc);
        }
    }
}
