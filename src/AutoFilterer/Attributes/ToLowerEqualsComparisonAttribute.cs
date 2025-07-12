using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace AutoFilterer.Attributes;

/// <summary>
/// This class generates equal query, for string fields.
/// </summary>
public class ToLowerEqualsComparisonAttribute : FilteringOptionsBaseAttribute
{
    public override Expression BuildExpression(ExpressionBuildContext context)
    {
        var equalsMethod = typeof(string).GetMethod(nameof(string.Equals), types: new[] { typeof(string) });

        var toLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), types: new Type[0]);

        var comparison = Expression.Equal(
                    Expression.Call(
                        method: equalsMethod,
                        instance: Expression.Call(method: toLowerMethod, instance: Expression.Property(context.ExpressionBody, context.TargetProperty.Name)
                            ),
                        arguments: new[] { Expression.Call(method: toLowerMethod, instance: context.FilterPropertyExpression) }),
                    Expression.Constant(true));

        return comparison;
    }
}
