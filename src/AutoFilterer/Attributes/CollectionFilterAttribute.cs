#if LEGACY_NAMESPACE
using AutoFilterer.Enums;
#endif
using AutoFilterer.Abstractions;
using AutoFilterer;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoFilterer.Attributes;

public class CollectionFilterAttribute : FilteringOptionsBaseAttribute
{
    public CollectionFilterAttribute()
    {
    }

    public CollectionFilterAttribute(CollectionFilterType filterOption)
    {
        this.FilterOption = filterOption;
    }

    public CollectionFilterType FilterOption { get; set; }

    public override Expression BuildExpression(Expression expressionBody, PropertyInfo targetProperty, PropertyInfo filterProperty, object value)
    {
        if (value is IFilter filter)
        {
            var type = targetProperty.PropertyType.GetGenericArguments().FirstOrDefault();
            var parameter = Expression.Parameter(type, "a");
            var innerLambda = Expression.Lambda(filter.BuildExpression(type, body: parameter), parameter);
            var prop = Expression.Property(expressionBody, targetProperty.Name);
            var methodInfo = typeof(Enumerable).GetMethods().LastOrDefault(x => x.Name == FilterOption.ToString());
            var method = methodInfo.MakeGenericMethod(type);

            expressionBody = Expression.Call(
                                        method: method,
                                        instance: null,
                                        arguments: new Expression[] { prop, innerLambda }
                );
        }
        return expressionBody;
    }
}
