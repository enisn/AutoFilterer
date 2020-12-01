using AutoFilterer.Abstractions;
using AutoFilterer.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Attributes
{
    public class CompareToAttribute : FilteringOptionsBaseAttribute
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

        public override Expression BuildExpression(Expression expressionBody, PropertyInfo targetProperty, PropertyInfo filterProperty, object value)
        {
            Expression innerExpression = null;
            foreach (var targetPropertyName in PropertyNames)
            {
                var _targetProperty = targetProperty.DeclaringType.GetProperty(targetPropertyName);
                if (_targetProperty == null)
                    continue;

                var bodyParameter = expressionBody;

                var expression = BuildExpressionForProperty(bodyParameter, _targetProperty, filterProperty, value);
                innerExpression = innerExpression.Combine(expression, CombineWith);
            }

            return innerExpression;
        }

        public virtual Expression BuildExpressionForProperty(Expression expressionBody, PropertyInfo targetProperty, PropertyInfo filterProperty, object value)
        {
            var attribute = filterProperty.GetCustomAttributes<FilteringOptionsBaseAttribute>().FirstOrDefault(x => !(x is CompareToAttribute));

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
