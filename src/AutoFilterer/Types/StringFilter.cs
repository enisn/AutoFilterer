using AutoFilterer.Abstractions;
using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoFilterer.Types
{
    public class StringFilter : IFilterableType
    {
        /// <summary>
        /// Provides parameter for equal operator '==' in query.
        /// </summary>
        public string Eq { get; set; }

        /// <summary>
        /// Provides parameter to not equal operator '!=' in query.
        /// </summary>
        public string Not { get; set; }

        /// <summary>
        /// Provides parameter to String.Equals method query.
        /// </summary>
        public new string Equals { get; set; }

        /// <summary>
        /// Provides parameter to String.Conains method query.
        /// </summary>
        public string Contains { get; set; }

        /// <summary>
        /// Provides parameter to String.StartsWith method query.
        /// </summary>
        public string StartsWith { get; set; }

        /// <summary>
        /// Provides parameter to String.EndsWith method query.
        /// </summary>
        public string EndsWith { get; set; }

        public virtual CombineType CombineWith { get; set; }

        /// <summary>
        /// Gets or sets comparison type of strings. Default is InvariantCultureIgnoreCase.
        /// </summary>
        public virtual StringComparison? Compare { get; set; }

        public Expression BuildExpression(Expression expressionBody, PropertyInfo targetProperty, PropertyInfo filterProperty, object value)
        {
            Expression expression = null;

            if (Eq != null)
                expression = Combine(expression, new OperatorComparisonAttribute(OperatorType.Equal).BuildExpression(expressionBody, targetProperty, filterProperty, Eq));

            if (Not != null)
                expression = Combine(expression, new OperatorComparisonAttribute(OperatorType.NotEqual).BuildExpression(expressionBody, targetProperty, filterProperty, Not));

            if (Equals != null)            
                expression = Combine(expression, new StringFilterOptionsAttribute(StringFilterOption.Equals) { Comparison = Compare }.BuildExpression(expressionBody, targetProperty, filterProperty, this.Equals));            

            if (Contains != null)
                expression = Combine(expression, new StringFilterOptionsAttribute(StringFilterOption.Contains) { Comparison = Compare }.BuildExpression(expressionBody, targetProperty, filterProperty, Contains));

            if (StartsWith != null)
                expression = Combine(expression, new StringFilterOptionsAttribute(StringFilterOption.StartsWith) { Comparison = Compare }.BuildExpression(expressionBody, targetProperty, filterProperty, StartsWith));

            if (EndsWith != null)
                expression = Combine(expression, new StringFilterOptionsAttribute(StringFilterOption.EndsWith) { Comparison = Compare }.BuildExpression(expressionBody, targetProperty, filterProperty, EndsWith));

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
