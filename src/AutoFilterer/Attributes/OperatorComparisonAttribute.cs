#if LEGACY_NAMESPACE
using AutoFilterer.Enums;
#endif
using AutoFilterer.Extensions;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoFilterer.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class OperatorComparisonAttribute : FilteringOptionsBaseAttribute
{
    public OperatorComparisonAttribute(OperatorType operatorType)
    {
        this.OperatorType = operatorType;
    }

    public OperatorType OperatorType { get; }

    public override Expression BuildExpression(ExpressionBuildContext context)
    {
        var prop = Expression.Property(context.ExpressionBody, context.TargetProperty.Name);

        var filterProp = BuildFilterExpression(context);

        var targetIsNullable = context.TargetProperty.PropertyType.IsNullable() || context.TargetProperty.PropertyType == typeof(string);

        if (context.TargetProperty.PropertyType.IsNullable())
        {
            prop = Expression.Property(prop, nameof(Nullable<bool>.Value));
        }

        switch (OperatorType)
        {
            case OperatorType.Equal:
                return Expression.Equal(prop, filterProp);
            case OperatorType.NotEqual:
                return Expression.NotEqual(prop, filterProp);
            case OperatorType.GreaterThan:
                return Expression.GreaterThan(prop, filterProp);
            case OperatorType.GreaterThanOrEqual:
                return Expression.GreaterThanOrEqual(prop, filterProp);
            case OperatorType.LessThan:
                return Expression.LessThan(prop, filterProp);
            case OperatorType.LessThanOrEqual:
                return Expression.LessThanOrEqual(prop, filterProp);
            case OperatorType.IsNull:
                return targetIsNullable ? Expression.Equal(Expression.Property(context.ExpressionBody, context.TargetProperty.Name), Expression.Constant(null)) : null;
            case OperatorType.IsNotNull:
                return targetIsNullable ? Expression.Not(Expression.Equal(Expression.Property(context.ExpressionBody, context.TargetProperty.Name), Expression.Constant(null))) : null;
        }

        return Expression.Empty();
    }

    #region Static

    public static OperatorComparisonAttribute Equal { get; }
    public static OperatorComparisonAttribute NotEqual { get; }
    public static OperatorComparisonAttribute GreaterThan { get; }
    public static OperatorComparisonAttribute GreaterThanOrEqual { get; }
    public static OperatorComparisonAttribute LessThan { get; }
    public static OperatorComparisonAttribute LessThanOrEqual { get; }
    public static OperatorComparisonAttribute IsNull { get; }
    public static OperatorComparisonAttribute IsNotNull { get; }

    static OperatorComparisonAttribute()
    {
        Equal = new OperatorComparisonAttribute(OperatorType.Equal);
        NotEqual = new OperatorComparisonAttribute(OperatorType.NotEqual);
        GreaterThan = new OperatorComparisonAttribute(OperatorType.GreaterThan);
        GreaterThanOrEqual = new OperatorComparisonAttribute(OperatorType.GreaterThanOrEqual);
        LessThan = new OperatorComparisonAttribute(OperatorType.LessThan);
        LessThanOrEqual = new OperatorComparisonAttribute(OperatorType.LessThanOrEqual);
        IsNull = new OperatorComparisonAttribute(OperatorType.IsNull);
        IsNotNull = new OperatorComparisonAttribute(OperatorType.IsNotNull);
    }

    #endregion
}