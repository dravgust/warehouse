using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Vayosoft.Core.Specifications
{
    public interface IFilteringSpecification<T>
    {
        string FilterString { get; }
        ICollection<Expression<Func<T, object>>> FilterBy { get; }
    }
}
