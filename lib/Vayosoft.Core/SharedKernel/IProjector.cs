using System.Linq;

namespace Vayosoft.Core.SharedKernel
{
    public interface IProjector
    {
        IQueryable<TReturn> Project<TSource, TReturn>(IQueryable<TSource> queryable);
    }
}
