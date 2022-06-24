using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Vayosoft.Core.SharedKernel.Specifications
{
    public class EntitySpecification<T> : IEntitySpecification<T>
    {
        public EntitySpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        public Expression<Func<T, bool>> Criteria { get; }
        public List<Expression<Func<T, object>>> Includes { get; } = new();
        public List<string> IncludeStrings { get; } = new();

        public void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        public void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }
    }
}
