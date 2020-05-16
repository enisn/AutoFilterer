using AutoFilterer.Abstractions;
using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using System;
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
                expression = Combine(expression, new OperatorComparisonAttribute(OperatorType.Equal).BuildExpression(expressionBody, targetProperty, filterProperty, Eq));

            if (Gt != null)
                expression = Combine(expression, new OperatorComparisonAttribute(OperatorType.GreaterThan).BuildExpression(expressionBody, targetProperty, filterProperty, Gt));

            if (Lt != null)
                expression = Combine(expression, new OperatorComparisonAttribute(OperatorType.LessThan).BuildExpression(expressionBody, targetProperty, filterProperty, Lt));

            if (Gte != null)
                expression = Combine(expression, new OperatorComparisonAttribute(OperatorType.GreaterThanOrEqual).BuildExpression(expressionBody, targetProperty, filterProperty, Gte));

            if (Lte != null)
                expression = Combine(expression, new OperatorComparisonAttribute(OperatorType.LessThanOrEqual).BuildExpression(expressionBody, targetProperty, filterProperty, Lte));

            if (Not != null)
                expression = Combine(expression, new OperatorComparisonAttribute(OperatorType.NotEqual).BuildExpression(expressionBody, targetProperty, filterProperty, Not));

            return expression;
        }

        private protected virtual Expression Combine(Expression left, Expression right)
        {
            if (left == null)
                return right;
            if (right == null)
                return left;

            if (left is ParameterExpression || left is MemberExpression)
                return right;
            if (right is ParameterExpression || right is MemberExpression)
                return left;

            switch (this.CombineWith)
            {
                case CombineType.And:
                    return Expression.And(left, right);
                case CombineType.Or:
                    return Expression.Or(left, right);
                default:
                    return right;
            }
        }
    }
}
