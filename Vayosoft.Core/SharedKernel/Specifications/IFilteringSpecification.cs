
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Vayosoft.Core.SharedKernel.Specifications
{
    public interface IFilteringSpecification<T>
    {
        string FilterString { get; }
        ICollection<Expression<Func<T, object>>> FilterBy { get; }
    }
}
