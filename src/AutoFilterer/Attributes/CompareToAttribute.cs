using AutoFilterer.Abstractions;
using AutoFilterer.Extensions;
using System;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoFilterer.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CompareToAttribute : Attribute, IPropertyExpressionable
    {
        public CompareToAttribute(params string[] propertyNames)
        {
            PropertyNames = propertyNames;
        }

        public string[] PropertyNames { get; set; }

        /// <summary>
        /// Gets or sets CombineType parameter. All properties are combined with 'Or' by default.
        /// </summary>
        public CombineType CombineWith { get; set; } = CombineType.Or;

        public virtual Expression CompareAndBuildExpression(Expression expressionBody, Type targetType, PropertyInfo filterProperty, object value)
        {
            Expression innerExpression = null;
            foreach (var targetPropertyName in PropertyNames)
            {
                var targetProperty = targetType.GetProperty(targetPropertyName);
                if (targetProperty == null)
                    continue;

                var bodyParameter = expressionBody;

                var expression = BuildExpression(bodyParameter, targetProperty, filterProperty, value);
                innerExpression = innerExpression.Combine(expression, CombineWith);
            }

            return innerExpression;
        }

        public virtual Expression BuildExpression(Expression expressionBody, PropertyInfo targetProperty, PropertyInfo filterProperty, object value)
        {
            var attribute = filterProperty.GetCustomAttribute<FilteringOptionsBaseAttribute>();

            if (attribute != null)
            {
                return attribute.BuildExpression(expressionBody, targetProperty, filterProperty, value);
            }

            if (value is IFilter filter)
            {
                if (typeof(ICollection).IsAssignableFrom(targetProperty.PropertyType) || (targetProperty.PropertyType.IsConstructedGenericType && typeof(IEnumerable).IsAssignableFrom(targetProperty.PropertyType)))
                {
                    var collectionAttribute = new CollectionFilterAttribute();
                    return collectionAttribute.BuildExpression(expressionBody, targetProperty, filterProperty, value);
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
                return new ArraySearchFilterAttribute().BuildExpression(expressionBody, targetProperty, filterProperty, value);
            }
            else
            {
                return new OperatorComparisonAttribute(OperatorType.Equal).BuildExpression(expressionBody, targetProperty, filterProperty, value);
            }
        }
    }
}
