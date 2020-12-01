using AutoFilterer.Abstractions;
using AutoFilterer.Attributes;
using AutoFilterer.Extensions;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoFilterer.Types
{
    public class OperatorFilter<T> : IFilterableType
        where T : struct
    {
        public T? Eq { get; set; }
        public T? Not { get; set; }
        public T? Gt { get; set; }
        public T? Lt { get; set; }
        public T? Gte { get; set; }
        public T? Lte { get; set; }

        public CombineType CombineWith { get; set; }
        public Expression BuildExpression(Expression expressionBody, PropertyInfo targetProperty, PropertyInfo filterProperty, object value)
        {
            Expression expression = null;

            if (Eq != null)
                expression = expression.Combine( new OperatorComparisonAttribute(OperatorType.Equal).BuildExpression(expressionBody, targetProperty, filterProperty, Eq), CombineWith);

            if (Gt != null)
                expression = expression.Combine( new OperatorComparisonAttribute(OperatorType.GreaterThan).BuildExpression(expressionBody, targetProperty, filterProperty, Gt), CombineWith);

            if (Lt != null)
                expression = expression.Combine( new OperatorComparisonAttribute(OperatorType.LessThan).BuildExpression(expressionBody, targetProperty, filterProperty, Lt), CombineWith);

            if (Gte != null)
                expression = expression.Combine( new OperatorComparisonAttribute(OperatorType.GreaterThanOrEqual).BuildExpression(expressionBody, targetProperty, filterProperty, Gte), CombineWith);

            if (Lte != null)
                expression = expression.Combine( new OperatorComparisonAttribute(OperatorType.LessThanOrEqual).BuildExpression(expressionBody, targetProperty, filterProperty, Lte), CombineWith);

            if (Not != null)
                expression = expression.Combine( new OperatorComparisonAttribute(OperatorType.NotEqual).BuildExpression(expressionBody, targetProperty, filterProperty, Not), CombineWith);

            return expression;
        }
    }
}
