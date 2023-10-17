using AutoFilterer.Abstractions;
using AutoFilterer.Extensions;
using System;
using System.Linq.Expressions;

namespace AutoFilterer.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public abstract class FilteringOptionsBaseAttribute : Attribute, IFilterableType
{
    public abstract Expression BuildExpression(ExpressionBuildContext context);

    protected Expression BuildFilterExpression(ExpressionBuildContext context)
    {
        var filterProp = context.FilterPropertyExpression;

        if (context.FilterProperty is null)
        {
            return Expression.Constant(context.FilterObjectPropertyValue);
        }

        if (context.FilterProperty.PropertyType.IsNullable())
        {
            filterProp = Expression.Property(filterProp, nameof(Nullable<bool>.Value));
        }

        return filterProp;
    }
}
