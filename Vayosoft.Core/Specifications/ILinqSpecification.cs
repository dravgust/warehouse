using System.Linq;

namespace Vayosoft.Core.Specifications
{
    public interface ILinqSpecification<T>
        where T : class
    {
        IQueryable<T> Apply(IQueryable<T> query);
    }
}
