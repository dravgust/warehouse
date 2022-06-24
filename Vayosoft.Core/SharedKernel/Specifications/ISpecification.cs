using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Vayosoft.Core.SharedKernel.Specifications
{
    public interface ISpecification<in T>
    {
        bool IsSatisfiedBy(T o);
    }

    public interface IEntitySpecification<T>
    {
        Expression<Func<T, bool>> Criteria { get; }
        List<Expression<Func<T, object>>> Includes { get; }
        List<string> IncludeStrings { get; }
    }
}