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

        public Func<string, string, bool> FilterRuleFunc { get; set; }

        public override Expression<Func<T,bool>> BuildExpression<T>(PropertyInfo property, object value)
        {
            var parameter = Expression.Parameter(property.DeclaringType, "x");
            var method = typeof(string).GetMethod(Option.ToString(), types: new[] { typeof(string) });

            var comparison = Expression.Equal(
                        Expression.Call(
                            method: method,
                            instance: Expression.Property(parameter, property.Name),
                            arguments: Expression.Constant(value)),
                        Expression.Constant(true)
                );

            return Expression.Lambda<Func<T, bool>>(comparison, parameter);
        }
    }
}
