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
    public virtual Expression BuildExpression(ExpressionBuildContext context)
    {
        Expression expression = null;

        if (Eq != null)
        {
            expression = expression.Combine(
                OperatorComparisonAttribute.Equal.BuildExpression(
                    ContextFor(context, nameof(Eq), Eq)
                ),
                CombineWith);
        }

        if (Gt != null)
            expression = expression.Combine(OperatorComparisonAttribute.GreaterThan.BuildExpression(ContextFor(context, nameof(Gt), Gt)), CombineWith);

        if (Lt != null)
            expression = expression.Combine(OperatorComparisonAttribute.LessThan.BuildExpression(ContextFor(context, nameof(Lt), Lt)), CombineWith);

        if (Gte != null)
            expression = expression.Combine(OperatorComparisonAttribute.GreaterThanOrEqual.BuildExpression(ContextFor(context, nameof(Gte), Gte)), CombineWith);

        if (Lte != null)
            expression = expression.Combine(OperatorComparisonAttribute.LessThanOrEqual.BuildExpression(ContextFor(context, nameof(Lte), Lte)), CombineWith);

        if (Not != null)
            expression = expression.Combine(OperatorComparisonAttribute.NotEqual.BuildExpression(ContextFor(context, nameof(Not), Not)), CombineWith);

        if (IsNull != null)
        {
            if (IsNull.Value)
            {
                expression = expression.Combine(OperatorComparisonAttribute.IsNull.BuildExpression(ContextFor(context, nameof(IsNull), null)), CombineWith);
            }
            else
            {
                expression = expression.Combine(OperatorComparisonAttribute.IsNotNull.BuildExpression(ContextFor(context, nameof(IsNull), null)), CombineWith);
            }
        }
        
        if (IsNotNull != null)
        {
            if (IsNotNull.Value)
            {
                expression = expression.Combine(OperatorComparisonAttribute.IsNotNull.BuildExpression(ContextFor(context, nameof(IsNotNull), null)), CombineWith);
            }
            else
            {
                expression = expression.Combine(OperatorComparisonAttribute.IsNull.BuildExpression(ContextFor(context, nameof(IsNotNull), null)), CombineWith);
            }
        }

        return expression;
    }

    private ExpressionBuildContext ContextFor(ExpressionBuildContext originalContext, string propertyName, T? value)
    {
        var innerProperty = originalContext.FilterProperty.PropertyType.GetProperty(propertyName);
        var innerPropertyExpression = Expression.Property(originalContext.FilterPropertyExpression, innerProperty);

        return new ExpressionBuildContext(
            originalContext.ExpressionBody,
            originalContext.TargetProperty,
            innerProperty,
            innerPropertyExpression,
            originalContext.FilterObject,
            value);
    }
}
