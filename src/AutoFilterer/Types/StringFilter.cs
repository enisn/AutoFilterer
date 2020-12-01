using AutoFilterer.Abstractions;
using AutoFilterer.Attributes;
using AutoFilterer.Extensions;
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
                expression = expression.Combine(new OperatorComparisonAttribute(OperatorType.Equal).BuildExpression(expressionBody, targetProperty, filterProperty, Eq), CombineWith);

            if (Not != null)
                expression = expression.Combine(new OperatorComparisonAttribute(OperatorType.NotEqual).BuildExpression(expressionBody, targetProperty, filterProperty, Not), CombineWith);

            if (Equals != null)            
                expression = expression.Combine(new StringFilterOptionsAttribute(StringFilterOption.Equals) { Comparison = Compare }.BuildExpression(expressionBody, targetProperty, filterProperty, Equals), CombineWith);            

            if (Contains != null)
                expression = expression.Combine( new StringFilterOptionsAttribute(StringFilterOption.Contains) { Comparison = Compare }.BuildExpression(expressionBody, targetProperty, filterProperty, Contains), CombineWith);

            if (StartsWith != null)
                expression = expression.Combine( new StringFilterOptionsAttribute(StringFilterOption.StartsWith) { Comparison = Compare }.BuildExpression(expressionBody, targetProperty, filterProperty, StartsWith), CombineWith);

            if (EndsWith != null)
                expression = expression.Combine(new StringFilterOptionsAttribute(StringFilterOption.EndsWith) { Comparison = Compare }.BuildExpression(expressionBody, targetProperty, filterProperty, EndsWith), CombineWith);

            return expression;
        }
    }
}
