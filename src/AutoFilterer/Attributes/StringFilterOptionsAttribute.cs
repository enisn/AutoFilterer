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

    public override Expression BuildExpression(Expression expressionBody, PropertyInfo targetProperty, PropertyInfo filterProperty, object value)
    {
        if (Comparison == null)
            return BuildExpressionWithoutComparison(this.Option, expressionBody, targetProperty, value);
        else
            return BuildExpressionWithComparison(this.Option, expressionBody, targetProperty, value);
    }

    private Expression BuildExpressionWithComparison(StringFilterOption option, Expression expressionBody, PropertyInfo property, object value)
    {
        var method = typeof(string).GetMethod(option.ToString(), types: new[] { typeof(string), typeof(StringComparison) });

        var comparison = Expression.Call(
                              method: method,
                              instance: Expression.Property(expressionBody, property.Name),
                              arguments: new[] { Expression.Constant(value), Expression.Constant(Comparison) });

        return comparison;
    }

    private Expression BuildExpressionWithoutComparison(StringFilterOption option, Expression expressionBody, PropertyInfo property, object value)
    {
        var method = typeof(string).GetMethod(option.ToString(), types: new[] { typeof(string) });

        var comparison = Expression.Call(
                            method: method,
                            instance: Expression.Property(expressionBody, property.Name),
                            arguments: new[] { Expression.Constant(value) });

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
