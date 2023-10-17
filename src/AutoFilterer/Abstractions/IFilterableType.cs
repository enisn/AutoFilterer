using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoFilterer.Abstractions;

/// <summary>
/// Any property type which is able to <see cref="BuildExpression(Expression, PropertyInfo, PropertyInfo, MemberExpression)"/> over source property.
/// <list type="table">
/// <item>
/// You can create new Complex Types via implementing this interface. It'll be automatically called if defined in an object which is implements <see cref="IFilter"/>.
/// </item>
/// </list>
/// </summary>
public interface IFilterableType
{
    Expression BuildExpression(ExpressionBuildContext context);
}
