#if LEGACY_NAMESPACE
using AutoFilterer.Enums;
#endif
using AutoFilterer.Abstractions;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
            for (int i = 0; i < PropertyNames.Length; i++)
            {
                var targetPropertyName = PropertyNames[i];
                var _targetProperty = targetProperty.DeclaringType.GetProperty(targetPropertyName);

                expressionBody = BuildExpressionForProperty(expressionBody, _targetProperty, filterProperty, value);
            }

            return expressionBody;
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
