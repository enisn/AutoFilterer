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
    public class StringFilterOptionsAttribute : Attribute, IFilteringOptions<string>
    {
        public StringFilterOptionsAttribute(StringFilterOption option)
        {
            this.Option = option;
        }

        public StringFilterOption Option { get; set; }

        private Func<string, string, bool> CombineIfNotNull(Func<string, string, bool> func1, Func<string, string, bool> func2)
        {
            if (func1 != null && func2 != null)
                return (s, p) => func1(s,p) || func2(s,p);

            if (func1 == null)
                return func2;

            if (func2 == null)
                return func1;

            return default;
        }
        public Func<string, string, bool> FilterRuleFunc { get; set; }

        public Expression<Func<T,bool>> BuildExpression<T>(PropertyInfo property, string value)
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

        public Expression<Func<T,bool>> BuildContainsExpression<T>(MemberExpression memberExp, object comparedValue)
        {
            var parameter = Expression.Parameter(memberExp.Member.DeclaringType, "x");
            var method = typeof(string).GetMethod("Contains", types: new[] { typeof(string) });

            var comparison = Expression.Equal(
                        Expression.Call(
                            method: method,
                            instance: memberExp,
                            arguments: Expression.Constant(comparedValue)),
                        Expression.Constant(true)
                );

            return Expression.Lambda<Func<T, bool>>(comparison, parameter);
        }
    }
}
