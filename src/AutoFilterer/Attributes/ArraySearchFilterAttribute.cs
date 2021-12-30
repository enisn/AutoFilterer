using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoFilterer.Attributes;

public class ArraySearchFilterAttribute : FilteringOptionsBaseAttribute
{
    public override Expression BuildExpression(Expression expressionBody, PropertyInfo targetProperty, PropertyInfo filterProperty, object value)
    {
        if (value is ICollection list && list.Count == 0)
            return Expression.Constant(true);
        var type = targetProperty.PropertyType;
        var prop = Expression.Property(expressionBody, targetProperty.Name);

        var containsMethod = typeof(Enumerable).GetMethods().FirstOrDefault(x => x.Name == nameof(Enumerable.Contains)).MakeGenericMethod(type);

        var containsExpression = Expression.Call(
                                                method: containsMethod,
                                                arguments: new Expression[]
                                                {
                                                        Expression.Constant(value),
                                                        Expression.Property(expressionBody, targetProperty.Name)
                                                });

        return containsExpression;
    }
}
