using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoFilterer.Attributes;

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public class IgnoreFilterAttribute : FilteringOptionsBaseAttribute
{
    public override Expression BuildExpression(Expression expressionBody, PropertyInfo targetProperty, PropertyInfo filterProperty, object value)
    {
        return expressionBody;
    }
}
