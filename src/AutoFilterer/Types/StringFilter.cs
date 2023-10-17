#if LEGACY_NAMESPACE
using AutoFilterer.Enums;
#endif
using AutoFilterer.Abstractions;
using AutoFilterer.Attributes;
using AutoFilterer.Extensions;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoFilterer.Types;

public class StringFilter : IFilterableType
{
    /// <summary>
    /// Provides parameter for equal operator '==' in query.
    /// </summary>
    public virtual string Eq { get; set; }

    /// <summary>
    /// Provides parameter to not equal operator '!=' in query.
    /// </summary>
    public virtual string Not { get; set; }

    /// <summary>
    /// Provides parameter to String.Equals method query.
    /// </summary>
    public virtual new string Equals { get; set; }

    /// <summary>
    /// Provides parameter to String.Contains method query.
    /// </summary>
    public virtual string Contains { get; set; }
    
    /// <summary>
    /// Provides parameter to !String.Contains method query.
    /// </summary>
    public virtual string NotContains { get; set; }

    /// <summary>
    /// Provides parameter to String.StartsWith method query.
    /// </summary>
    public virtual string StartsWith { get; set; }
    
    /// <summary>
    /// Provides parameter to !String.StartsWith method query.
    /// </summary>
    public virtual string NotStartsWith { get; set; }

    /// <summary>
    /// Provides parameter to String.EndsWith method query.
    /// </summary>
    public virtual string EndsWith { get; set; }
    
    /// <summary>
    /// Provides parameter to !String.EndsWith method query.
    /// </summary>
    public virtual string NotEndsWith { get; set; }
    
    /// <summary>
    /// Provides parameter to check is null.
    /// </summary>
    public virtual bool? IsNull { get; set; }
    
    /// <summary>
    /// Provides parameter to check is not null.
    /// </summary>
    public virtual bool? IsNotNull { get; set; }
    
    /// <summary>
    /// Provides parameter to check is empty string.
    /// </summary>
    public virtual bool? IsEmpty { get; set; }
    
    /// <summary>
    /// Provides parameter to check is not empty string.
    /// </summary>
    public virtual bool? IsNotEmpty { get; set; }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    public virtual CombineType CombineWith { get; set; }

    /// <summary>
    /// Gets or sets comparison type of strings. Default is InvariantCultureIgnoreCase.
    /// </summary>
    public virtual StringComparison? Compare { get; set; }

    public virtual Expression BuildExpression(ExpressionBuildContext context)
    {
        Expression expression = null;

        if (Eq != null)
            expression = expression.Combine(OperatorComparisonAttribute.Equal.BuildExpression(ContextFor(context, nameof(Eq), Eq)), CombineWith);

        if (Not != null)
            expression = expression.Combine(OperatorComparisonAttribute.NotEqual.BuildExpression(ContextFor(context, nameof(Not), Not)), CombineWith);

        if (IsNull != null)
            expression = expression.Combine(OperatorComparisonAttribute.IsNull.BuildExpression(ContextFor(context, nameof(IsNull), null)), CombineWith);

        if (IsNotNull != null)
            expression = expression.Combine(OperatorComparisonAttribute.IsNotNull.BuildExpression(ContextFor(context, nameof(IsNotNull), null)), CombineWith);
        
        if (Equals != null)
            expression = expression.Combine(new StringFilterOptionsAttribute(StringFilterOption.Equals) { Comparison = Compare }.BuildExpression(ContextFor(context, nameof(Equals), Equals)), CombineWith);

        if (Contains != null)
            expression = expression.Combine(new StringFilterOptionsAttribute(StringFilterOption.Contains) { Comparison = Compare }.BuildExpression(ContextFor(context, nameof(Contains), Contains)), CombineWith);

        if (NotContains != null)
            expression = expression.Combine(Expression.Not(new StringFilterOptionsAttribute(StringFilterOption.Contains) { Comparison = Compare }.BuildExpression(ContextFor(context, nameof(NotContains), NotContains))), CombineWith);

        if (StartsWith != null)
            expression = expression.Combine(new StringFilterOptionsAttribute(StringFilterOption.StartsWith) { Comparison = Compare }.BuildExpression(ContextFor(context, nameof(StartsWith), StartsWith)), CombineWith);

        if (NotStartsWith != null)
            expression = expression.Combine(Expression.Not(new StringFilterOptionsAttribute(StringFilterOption.StartsWith) { Comparison = Compare }.BuildExpression(ContextFor(context, nameof(NotStartsWith), NotStartsWith))), CombineWith);

        if (EndsWith != null)
            expression = expression.Combine(new StringFilterOptionsAttribute(StringFilterOption.EndsWith) { Comparison = Compare }.BuildExpression(ContextFor(context, nameof(EndsWith), EndsWith)), CombineWith);
        
        if (NotEndsWith != null)
            expression = expression.Combine(Expression.Not(new StringFilterOptionsAttribute(StringFilterOption.EndsWith) { Comparison = Compare }.BuildExpression(ContextFor(context, nameof(NotEndsWith), NotEndsWith))), CombineWith);

        if (IsEmpty != null)
        {
            if (IsEmpty.Value)
            { 
                expression = expression.Combine(new StringFilterOptionsAttribute(StringFilterOption.Equals) { Comparison = Compare }
                .BuildExpression(ContextForConstant(context, string.Empty)), CombineWith);
            }
            else
            {
                expression = expression.Combine(Expression.Not(new StringFilterOptionsAttribute(StringFilterOption.Equals) { Comparison = Compare }
                .BuildExpression(ContextForConstant(context, string.Empty))), CombineWith);
            }
        }
        
        if (IsNotEmpty != null)
        {
            if (IsNotEmpty.Value)
            {
                expression = expression.Combine(Expression.Not(new StringFilterOptionsAttribute(StringFilterOption.Equals) { Comparison = Compare }
                .BuildExpression(ContextForConstant(context, string.Empty))), CombineWith);
            }
            else
            {
                expression = expression.Combine(new StringFilterOptionsAttribute(StringFilterOption.Equals) { Comparison = Compare }
                .BuildExpression(ContextForConstant(context, string.Empty)), CombineWith);
            }
        }
        
        return expression;
    }

    private ExpressionBuildContext ContextFor(ExpressionBuildContext originalContext, string propertyName, string value)
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

    private ExpressionBuildContext ContextForConstant(ExpressionBuildContext originalContext, string value)
    {
        return new ExpressionBuildContext(
            originalContext.ExpressionBody,
            originalContext.TargetProperty,
            null,
            Expression.Constant(value),
            originalContext.FilterObject,
            value);
    }
}
