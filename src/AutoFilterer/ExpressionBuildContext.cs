using AutoFilterer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace AutoFilterer;
public class ExpressionBuildContext
{
    public ExpressionBuildContext(
        Expression expressionBody,
        PropertyInfo targetProperty,
        PropertyInfo filterProperty,
        Expression filterPropertyExpression,
        IFilter filterObject,
        object propertyValue)
    {
        ExpressionBody = expressionBody;
        TargetProperty = targetProperty;
        FilterProperty = filterProperty;
        FilterPropertyExpression = filterPropertyExpression;
        FilterObject = filterObject;
        FilterObjectPropertyValue = propertyValue;
    }

    public Expression ExpressionBody { get; }

    public PropertyInfo TargetProperty { get; }

    public PropertyInfo FilterProperty { get; }

    public Expression FilterPropertyExpression { get; }

    public IFilter FilterObject { get; }

    public object FilterObjectPropertyValue { get; }
}
