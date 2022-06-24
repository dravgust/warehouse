using Vayosoft.Core.SharedKernel.Entities;

namespace Vayosoft.Core.SharedKernel.Models.Pagination
{
    public class SortByIdPagingModelModel<TEntity>: SortByIdPagingModelModel<TEntity, long>
        where TEntity : class, IEntity<long>
    {
        public SortByIdPagingModelModel(int page, int take, SortOrder orderBy)
            : base(page, take, orderBy)
        {
        }
        
        public SortByIdPagingModelModel()
        {
        }
    }

    public class SortByIdPagingModelModel<TEntity, TKey>: PagingModelModelBase<TEntity, TKey> 
        where TEntity : class, IEntity<TKey>
    {
        public SortByIdPagingModelModel(int page, int take, SortOrder orderBy)
            : base(page, take, new Sorting<TEntity, TKey>(x => x.Id, orderBy))
        {
        }

        public SortByIdPagingModelModel()
        {
        }

        protected override Sorting<TEntity, TKey> BuildDefaultSorting()
        {
            return new Sorting<TEntity, TKey>(x => x.Id, SortOrder.Desc);
        }
    }
}
