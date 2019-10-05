using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoFilterer.Abstractions
{
    public interface IFilterableType
    {
        Expression BuildExpression(Expression expressionBody, PropertyInfo property, object value);
    }
}
