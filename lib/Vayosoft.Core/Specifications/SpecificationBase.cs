using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Vayosoft.Core.Specifications
{
    public class SpecificationBase<T> : ISpecification<T>
    {
        public SpecificationBase(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        public Expression<Func<T, bool>> Criteria { get; }
        public ICollection<Expression<Func<T, object>>> Includes { get; }
            = new List<Expression<Func<T, object>>>();
        public ICollection<string> IncludeStrings { get; }
            = new List<string>();

        public ICollection<Expression<Func<T, bool>>> WhereExpressions { get; }
            = new List<Expression<Func<T, bool>>>();

        public SpecificationBase<T> AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);

            return this;
        }

        public SpecificationBase<T> AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);

            return this;
        }
        public SpecificationBase<T> Where(Expression<Func<T, bool>> includeExpression)
        {
            WhereExpressions.Add(includeExpression);

            return this;
        }

        public SpecificationBase<T> WhereIf(bool condition, Expression<Func<T, bool>> includeExpression)
        {
            if (condition)
            {
                WhereExpressions.Add(includeExpression);
            }

            return this;
        }
    }
}
