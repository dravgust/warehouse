using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Vayosoft.Core.Specifications
{
    public interface ICriteriaSpecification<T>
    {
        Expression<Func<T, bool>> Criteria { get; }
        ICollection<Expression<Func<T, object>>> Includes { get; }
        ICollection<string> IncludeStrings { get; }
    }
}
