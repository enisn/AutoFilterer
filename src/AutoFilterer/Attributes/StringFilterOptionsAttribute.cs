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

        public StringFilterOption Option { get; set; }
        public StringComparison StringComparison { get; set; } = StringComparison.InvariantCultureIgnoreCase;

        public Func<string, string, bool> FilterRuleFunc { get; set; }

        public override Expression BuildExpression(Expression expressionBody, PropertyInfo property, object value)
        {
            var method = typeof(string).GetMethod(Option.ToString(), types: new[] { typeof(string), typeof(StringComparison) });

            var comparison = Expression.Equal(
                        Expression.Call(
                            method: method,
                            instance: Expression.Property(expressionBody, property.Name),
                            arguments: new[] { Expression.Constant(value), Expression.Constant(StringComparison) }),
                        Expression.Constant(true)
                );

            return comparison;
        }
    }
}
