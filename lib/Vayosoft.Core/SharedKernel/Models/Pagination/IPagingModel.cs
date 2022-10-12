namespace Vayosoft.Core.SharedKernel.Models.Pagination
{
    public interface IPagingModel
    {
        int Page { get; }

        int Size { get; }

        const int DefaultSize = 30;
    }

    public interface IPagingModel<TEntity, TSortKey> : IPagingModel
        where TEntity : class
    {
        Sorting<TEntity, TSortKey> OrderBy { get; }
    }
}