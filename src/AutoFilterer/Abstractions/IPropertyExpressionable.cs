using System.Linq.Expressions;
using System.Reflection;

namespace AutoFilterer.Abstractions
{
    public interface IPropertyExpressionable
    {
        Expression BuildExpression(Expression expressionBody, PropertyInfo targetProperty, PropertyInfo filterProperty, object value);
    }
}
