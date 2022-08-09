namespace Vayosoft.Core.SharedKernel.Models.Pagination
{
    public interface IPagingModel<TEntity, TSortKey>
        where TEntity : class
    {
        int Page { get; }

        int Size { get; }

        Sorting<TEntity, TSortKey> OrderBy { get; }
    }
}