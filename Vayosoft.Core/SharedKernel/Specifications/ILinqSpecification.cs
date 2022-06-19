using System.Linq;

namespace Vayosoft.Core.SharedKernel.Specifications
{
    public interface ILinqSpecification<T>
        where T : class
    {
        IQueryable<T> Apply(IQueryable<T> query);
    }
}
