#if LEGACY_NAMESPACE
using AutoFilterer.Enums;
#endif
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoFilterer.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class StringFilterOptionsAttribute : FilteringOptionsBaseAttribute
{
    public StringFilterOptionsAttribute(StringFilterOption option)
    {
        this.Option = option;
    }

    public StringFilterOptionsAttribute(StringFilterOption option, StringComparison comparison) : this(option)
    {
        this.Comparison = comparison;
    }

    public StringFilterOption Option { get; set; }

    public StringComparison? Comparison { get; set; }

    public override Expression BuildExpression(ExpressionBuildContext context)
    {
        if (Comparison == null)
        {
            return BuildExpressionWithoutComparison(this.Option, context);
        }
        else
        {
            return BuildExpressionWithComparison(this.Option, context);
        }
    }

    private Expression BuildExpressionWithComparison(StringFilterOption option, ExpressionBuildContext context)
    {
        var method = typeof(string).GetMethod(option.ToString(), types: new[] { typeof(string), typeof(StringComparison) });
        var filterProp = BuildFilterExpression(context);

        var comparison = Expression.Call(
                              method: method,
                              instance: Expression.Property(context.ExpressionBody, context.TargetProperty.Name),
                              arguments: new Expression[] { filterProp, Expression.Constant(Comparison) });

        return comparison;
    }

    private Expression BuildExpressionWithoutComparison(StringFilterOption option, ExpressionBuildContext context)
    {
        var method = typeof(string).GetMethod(option.ToString(), types: new[] { typeof(string) });

        var filterProp = BuildFilterExpression(context);

        var comparison = Expression.Call(
                            method: method,
                            instance: Expression.Property(context.ExpressionBody, context.TargetProperty.Name),
                            arguments: new[] { filterProp });

        return comparison;
    }

    #region Static
    public static StringFilterOptionsAttribute Contains { get; }
    public static StringFilterOptionsAttribute EndsWith { get; }
    public static StringFilterOptionsAttribute StartsWith { get; }
    static StringFilterOptionsAttribute()
    {
        Contains = new StringFilterOptionsAttribute(StringFilterOption.Contains);
        EndsWith = new StringFilterOptionsAttribute(StringFilterOption.EndsWith);
        StartsWith = new StringFilterOptionsAttribute(StringFilterOption.StartsWith);
    }
    #endregion
}
