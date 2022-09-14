namespace Vayosoft.Core.SharedKernel.Models.Pagination
{
    public interface IPagingModel<TEntity, TSortKey>
        where TEntity : class
    {
        int Page { get; }

        int PageSize { get; }

        Sorting<TEntity, TSortKey> OrderBy { get; }
    }
}