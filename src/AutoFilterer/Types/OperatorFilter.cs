#if LEGACY_NAMESPACE
using AutoFilterer.Enums;
#endif
using AutoFilterer.Abstractions;
using AutoFilterer.Attributes;
using AutoFilterer.Extensions;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoFilterer.Types;

public class OperatorFilter<T> : IFilterableType
    where T : struct
{
    public virtual T? Eq { get; set; }
    public virtual T? Not { get; set; }
    public virtual T? Gt { get; set; }
    public virtual T? Lt { get; set; }
    public virtual T? Gte { get; set; }
    public virtual T? Lte { get; set; }
    public virtual bool? IsNull { get; set; }
    public virtual bool? IsNotNull { get; set; }

    public virtual CombineType CombineWith { get; set; }
    public virtual Expression BuildExpression(Expression expressionBody, PropertyInfo targetProperty, PropertyInfo filterProperty, object value)
    {
        Expression expression = null;

        if (Eq != null)
            expression = expression.Combine(OperatorComparisonAttribute.Equal.BuildExpression(expressionBody, targetProperty, filterProperty, Eq), CombineWith);

        if (Gt != null)
            expression = expression.Combine(OperatorComparisonAttribute.GreaterThan.BuildExpression(expressionBody, targetProperty, filterProperty, Gt), CombineWith);

        if (Lt != null)
            expression = expression.Combine(OperatorComparisonAttribute.LessThan.BuildExpression(expressionBody, targetProperty, filterProperty, Lt), CombineWith);

        if (Gte != null)
            expression = expression.Combine(OperatorComparisonAttribute.GreaterThanOrEqual.BuildExpression(expressionBody, targetProperty, filterProperty, Gte), CombineWith);

        if (Lte != null)
            expression = expression.Combine(OperatorComparisonAttribute.LessThanOrEqual.BuildExpression(expressionBody, targetProperty, filterProperty, Lte), CombineWith);

        if (Not != null)
            expression = expression.Combine(OperatorComparisonAttribute.NotEqual.BuildExpression(expressionBody, targetProperty, filterProperty, Not), CombineWith);

        if (IsNull != null)
        {
            if (IsNull.Value)
            {
                expression = expression.Combine(OperatorComparisonAttribute.IsNull.BuildExpression(expressionBody, targetProperty, filterProperty, IsNull.Value), CombineWith);
            }
            else
            {
                expression = expression.Combine(OperatorComparisonAttribute.IsNotNull.BuildExpression(expressionBody, targetProperty, filterProperty, IsNull.Value), CombineWith);
            }
        }
        
        if (IsNotNull != null)
        {
            if (IsNotNull.Value)
            {
                expression = expression.Combine(OperatorComparisonAttribute.IsNotNull.BuildExpression(expressionBody, targetProperty, filterProperty, IsNotNull.Value), CombineWith);
            }
            else
            {
                expression = expression.Combine(OperatorComparisonAttribute.IsNull.BuildExpression(expressionBody, targetProperty, filterProperty, IsNotNull.Value), CombineWith);
            }
        }

        return expression;
    }
}
