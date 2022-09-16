namespace Vayosoft.Core.SharedKernel.Models.Pagination
{
    public interface IPagingModel
    {
        int Page { get; }

        int Size { get; }
    }

    public interface IPagingModel<TEntity, TSortKey> : IPagingModel
        where TEntity : class
    {
        Sorting<TEntity, TSortKey> OrderBy { get; }
    }
}