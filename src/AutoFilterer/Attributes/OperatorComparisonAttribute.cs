using AutoFilterer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Attributes
{
    public class OperatorComparisonAttribute : FilteringOptionsBaseAttribute
    {
        public OperatorComparisonAttribute(OperatorType operatorType)
        {
            this.OperatorType = operatorType;
        }

        public OperatorType OperatorType { get; }

        public override Expression BuildExpression(Expression expressionBody, PropertyInfo property, object value)
        {
            var prop = Expression.Property(expressionBody, property.Name);
            var param = Expression.Constant(value);

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
