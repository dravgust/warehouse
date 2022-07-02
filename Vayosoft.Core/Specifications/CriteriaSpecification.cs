using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Vayosoft.Core.Specifications
{
    public class CriteriaSpecification<T> : ICriteriaSpecification<T>
    {
        public CriteriaSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        public Expression<Func<T, bool>> Criteria { get; }
        public ICollection<Expression<Func<T, object>>> Includes { get; }
            = new List<Expression<Func<T, object>>>();
        public ICollection<string> IncludeStrings { get; }
            = new List<string>();

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
