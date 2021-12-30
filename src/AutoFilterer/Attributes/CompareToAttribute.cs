#if LEGACY_NAMESPACE
using AutoFilterer.Enums;
#endif
using AutoFilterer.Abstractions;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System;

namespace AutoFilterer.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class CompareToAttribute : FilteringOptionsBaseAttribute
{
    private Type filterableType;

    public CompareToAttribute(params string[] propertyNames)
    {
        PropertyNames = propertyNames;
    }

    public CompareToAttribute(Type filterableType, params string[] propertyNames)
    {
        FilterableType = filterableType;
        PropertyNames = propertyNames;
    }

    public string[] PropertyNames { get; set; }

    /// <summary>
    /// Gets or sets CombineType parameter. All properties are combined with 'Or' by default.
    /// </summary>
    public CombineType CombineWith { get; set; } = CombineType.Or;

    /// <summary>
    /// Type must implement <see cref="IFilterableType"/> and must has parameterless constructor.
    /// </summary>
    public Type FilterableType
    {
        get => filterableType;
        set
        {
            if (!typeof(IFilterableType).IsAssignableFrom(value))
            {
                throw new ArgumentException($"The {value.FullName} type must implement 'IFilterableType'", nameof(FilterableType));
            }

            filterableType = value;
        }
    }

    public override Expression BuildExpression(Expression expressionBody, PropertyInfo targetProperty, PropertyInfo filterProperty, object value)
    {
        for (int i = 0; i < PropertyNames.Length; i++)
        {
            var targetPropertyName = PropertyNames[i];
            var _targetProperty = targetProperty.DeclaringType.GetProperty(targetPropertyName);
            if (FilterableType != null)
            {
                expressionBody = ((IFilterableType)Activator.CreateInstance(FilterableType)).BuildExpression(expressionBody, _targetProperty, filterProperty, value);
            }
            else
            {
                expressionBody = BuildExpressionForProperty(expressionBody, _targetProperty, filterProperty, value);
            }
        }

        return expressionBody;
    }

    public virtual Expression BuildExpressionForProperty(Expression expressionBody, PropertyInfo targetProperty, PropertyInfo filterProperty, object value)
    {
        if (FilterableType != null)
        {
            return ((IFilterableType)Activator.CreateInstance(FilterableType)).BuildExpression(expressionBody, targetProperty, filterProperty, value);
        }

        var attribute = filterProperty.GetCustomAttributes<FilteringOptionsBaseAttribute>().FirstOrDefault(x => !(x is CompareToAttribute));

        if (attribute != null)
        {
            return attribute.BuildExpression(expressionBody, targetProperty, filterProperty, value);
        }

        return BuildDefaultExpression(expressionBody, targetProperty, filterProperty, value);
    }

    public virtual Expression BuildDefaultExpression(Expression expressionBody, PropertyInfo targetProperty, PropertyInfo filterProperty, object value)
    {
        if (value is IFilter filter)
        {
            if (typeof(ICollection).IsAssignableFrom(targetProperty.PropertyType) || (targetProperty.PropertyType.IsConstructedGenericType && typeof(IEnumerable).IsAssignableFrom(targetProperty.PropertyType)))
            {
                return Singleton<CollectionFilterAttribute>.Instance.BuildExpression(expressionBody, targetProperty, filterProperty, value);
            }
            else
            {
                var parameter = Expression.Property(expressionBody, targetProperty.Name);
                return filter.BuildExpression(targetProperty.PropertyType, parameter);
            }
        }

        if (value is IFilterableType filterableProperty)
        {
            return filterableProperty.BuildExpression(expressionBody, targetProperty, filterProperty, value);
        }
        else if (filterProperty.PropertyType.IsArray && !typeof(ICollection).IsAssignableFrom(targetProperty.PropertyType))
        {
            return Singleton<ArraySearchFilterAttribute>.Instance.BuildExpression(expressionBody, targetProperty, filterProperty, value);
        }
        else
        {
            return OperatorComparisonAttribute.Equal.BuildExpression(expressionBody, targetProperty, filterProperty, value);
        }
    }
}
