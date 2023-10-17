using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoFilterer.Attributes;

public class ArraySearchFilterAttribute : FilteringOptionsBaseAttribute
{
    public override Expression BuildExpression(ExpressionBuildContext context)
    {
        if (context.FilterProperty is ICollection list && list.Count == 0)
        {
            return Expression.Constant(true); // TODO: Make it better. Maybe return null? When null, it should be ignored and combined with another expressions.
        }

        var type = context.TargetProperty.PropertyType;
        var prop = Expression.Property(context.ExpressionBody, context.TargetProperty.Name);

        var containsMethod = typeof(Enumerable).GetMethods().FirstOrDefault(x => x.Name == nameof(Enumerable.Contains)).MakeGenericMethod(type);

        var containsExpression = Expression.Call(
                                                method: containsMethod,
                                                arguments: new Expression[]
                                                {
                                                        Expression.Property(Expression.Constant(context.FilterObject), context.FilterProperty),
                                                        Expression.Property(context.ExpressionBody, context.TargetProperty)
                                                });

        return containsExpression;
    }
}
