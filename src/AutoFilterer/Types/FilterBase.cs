#if LEGACY_NAMESPACE
using AutoFilterer.Enums;
#endif
using AutoFilterer.Abstractions;
using AutoFilterer.Attributes;
using AutoFilterer.Extensions;
using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoFilterer.Types;

/// <summary>
/// Base class of filter Data Transfer Objects.
/// </summary>
public class FilterBase : IFilter
{
    public static bool IgnoreExceptions { get; set; } = true;

    [IgnoreFilter]
    public virtual CombineType CombineWith { get; set; }

    public virtual IQueryable<TEntity> ApplyFilterTo<TEntity>(IQueryable<TEntity> query)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "x");

        var exp = BuildExpression(typeof(TEntity), parameter);
        if (exp == null)
            return query;

        if (exp is MemberExpression || exp is ParameterExpression)
            return query;

        var lambda = Expression.Lambda<Func<TEntity, bool>>(exp, parameter);
        return query.Where(lambda);
    }

    public virtual Expression BuildExpression(Type entityType, Expression body)
    {
        Expression finalExpression = body;
        var _type = this.GetType();
        foreach (var filterProperty in _type.GetProperties())
        {
            try
            {
                var filterPropertyValue = filterProperty.GetValue(this);
                var filterPropertyExpression = Expression.Property(Expression.Constant(this), filterProperty);

                if (filterPropertyValue == null || filterProperty.GetCustomAttribute<IgnoreFilterAttribute>() != null)
                {
                    continue;
                }

                var attributes = filterProperty.GetCustomAttributes<CompareToAttribute>(inherit: true);

                if (!attributes.Any())
                {
                    attributes = new[] { new CompareToAttribute(filterProperty.Name) };
                }

                Expression innerExpression = null;

                foreach (var attribute in attributes)
                {
                    foreach (var targetPropertyName in attribute.PropertyNames)
                    {
                        var bodyParameter = finalExpression is MemberExpression ? finalExpression : body;

                        if (!TryResolveTargetPath(entityType, bodyParameter, targetPropertyName, out var targetBody, out var targetProperty, out var nullGuard))
                            continue;

                        var expression = attribute.BuildExpressionForProperty(
                            new ExpressionBuildContext(targetBody, targetProperty, filterProperty, filterPropertyExpression, this, filterPropertyValue));

                        if (expression != null && nullGuard != null)
                        {
                            expression = Expression.AndAlso(nullGuard, expression);
                        }

                        innerExpression = innerExpression.Combine(expression, attribute.CombineWith);
                    }
                }

                var combined = finalExpression.Combine(innerExpression, CombineWith);
                finalExpression = combined.Combine(body, CombineWith);
            }
            catch (Exception ex)
            {
                if (!IgnoreExceptions)
                {
                    throw;
                }

                Debug.WriteLine(ex?.ToString());
            }
        }

        return finalExpression;
    }

    private static bool TryResolveTargetPath(Type entityType, Expression expressionBody, string targetPropertyName, out Expression targetBody, out PropertyInfo targetProperty, out Expression nullGuard)
    {
        targetBody = expressionBody;
        targetProperty = null;
        nullGuard = null;

        if (string.IsNullOrWhiteSpace(targetPropertyName))
        {
            return false;
        }

        var parts = targetPropertyName.Split('.');
        var currentType = entityType;
        var currentExpression = expressionBody;

        for (var i = 0; i < parts.Length; i++)
        {
            var property = currentType.GetProperty(parts[i]);
            if (property == null)
            {
                return false;
            }

            if (i == parts.Length - 1)
            {
                targetBody = currentExpression;
                targetProperty = property;
                return true;
            }

            currentExpression = Expression.Property(currentExpression, property);
            currentType = property.PropertyType;

            if (CanBeNull(property.PropertyType))
            {
                var notNull = Expression.NotEqual(currentExpression, Expression.Constant(null, property.PropertyType));
                nullGuard = nullGuard == null ? notNull : Expression.AndAlso(nullGuard, notNull);
            }
        }

        return false;
    }

    private static bool CanBeNull(Type type)
    {
        return !type.IsValueType || Nullable.GetUnderlyingType(type) != null;
    }
}
