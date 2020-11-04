using AutoFilterer.Enums;
using AutoFilterer.Extensions;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoFilterer.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class OperatorComparisonAttribute : FilteringOptionsBaseAttribute
    {
        public OperatorComparisonAttribute(OperatorType operatorType)
        {
            this.OperatorType = operatorType;
        }

        public OperatorType OperatorType { get; }

        public override Expression BuildExpression(Expression expressionBody, PropertyInfo targetProperty, PropertyInfo filterProperty, object value)
        {
            var prop = Expression.Property(expressionBody, targetProperty.Name);
            var param = Expression.Constant(value);

            if (targetProperty.PropertyType.IsNullable())
                prop = Expression.Property(prop, nameof(Nullable<bool>.Value));

            switch (OperatorType)
            {
                case OperatorType.Equal:
                    return Expression.Equal(prop, param);
                case OperatorType.NotEqual:
                    return Expression.NotEqual(prop, param);
                case OperatorType.GreaterThan:
                    return Expression.GreaterThan(prop, param);
                case OperatorType.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(prop, param);
                case OperatorType.LessThan:
                    return Expression.LessThan(prop, param);
                case OperatorType.LessThanOrEqual:
                    return Expression.LessThanOrEqual(prop, param);
            }

            return Expression.Empty();
        }
    }
}
