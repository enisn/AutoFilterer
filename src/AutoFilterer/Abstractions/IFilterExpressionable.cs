using System;
using System.Linq.Expressions;

namespace AutoFilterer.Abstractions
{
    public interface IFilterExpressionable
    {
        Expression BuildExpression(Type entityType, Expression body);
    }
}
