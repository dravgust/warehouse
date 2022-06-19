using Vayosoft.Core.SharedKernel.Entities;

namespace Vayosoft.Core.SharedKernel.Models.Pagination
{
    public class IdPaging<TEntity>: IdPaging<TEntity, ulong>
        where TEntity : class, IEntity<ulong>
    {
        public IdPaging(int page, int take, SortOrder orderBy)
            : base(page, take, orderBy)
        {
        }
        
        public IdPaging()
        {
        }
    }

    public class IdPaging<TEntity, TKey>: Paging<TEntity, TKey> 
        where TEntity : class, IEntity<TKey>
    {
        public IdPaging(int page, int take, SortOrder orderBy)
            : base(page, take, new Sorting<TEntity, TKey>(x => x.Id, orderBy))
        {
        }

        public IdPaging()
        {
        }

        protected override Sorting<TEntity, TKey> BuildDefaultSorting()
        {
            return new Sorting<TEntity, TKey>(x => x.Id, SortOrder.Desc);
        }
    }
}
