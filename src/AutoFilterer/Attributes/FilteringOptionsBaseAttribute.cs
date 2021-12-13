using AutoFilterer.Abstractions;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoFilterer.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public abstract class FilteringOptionsBaseAttribute : Attribute, IFilterableType
{
    public abstract Expression BuildExpression(Expression expressionBody, PropertyInfo targetProperty, PropertyInfo filterProperty, object value);
}
