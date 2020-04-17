using AutoFilterer.Abstractions;
using AutoFilterer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class StringFilterOptionsAttribute : FilteringOptionsBaseAttribute
    {
        public StringFilterOptionsAttribute(StringFilterOption option)
        {
            this.Option = option;
        }

        public StringFilterOptionsAttribute(StringFilterOption option, StringComparison comparison) : this(option)
        {
            this.Comparison = comparison;
        }

        public StringFilterOption Option { get; set; }

        public StringComparison? Comparison { get; set; }

        public override Expression BuildExpression(Expression expressionBody, PropertyInfo property, object value)
        {
            if (Comparison == null)
                return BuildExpressionWithoutComparison(this.Option, expressionBody, property, value);
            else
                return BuildExpressionWithComparison(this.Option, expressionBody, property, value);
        }

        private Expression BuildExpressionWithComparison(StringFilterOption option, Expression expressionBody, PropertyInfo property, object value)
        {
            var method = typeof(string).GetMethod(option.ToString(), types: new[] { typeof(string), typeof(StringComparison) });

            var comparison = Expression.Equal(
                      Expression.Call(
                          method: method,
                          instance: Expression.Property(expressionBody, property.Name),
                          arguments: new[] { Expression.Constant(value), Expression.Constant(Comparison) }),
                      Expression.Constant(true)
              );

            return comparison;
        }

        private Expression BuildExpressionWithoutComparison(StringFilterOption option, Expression expressionBody, PropertyInfo property, object value)
        {
            var method = typeof(string).GetMethod(option.ToString(), types: new[] { typeof(string) });

            var comparison = Expression.Equal(
                      Expression.Call(
                          method: method,
                          instance: Expression.Property(expressionBody, property.Name),
                          arguments: new[] { Expression.Constant(value) }),
                      Expression.Constant(true)
              );

            return comparison;
        }
    }
}
