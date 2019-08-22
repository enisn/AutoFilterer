using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoFilterer.Abstractions
{
    /// <summary>
    /// A base interface to build expressions.
    /// Visit https://github.com/enisn/AutoFilterer/wiki for more information.
    /// </summary>
    public interface IFilterableType
    {
        Expression BuildExpression(PropertyInfo property, object value);
        Expression<Func<TEntity, bool>> BuildExpression<TEntity>(PropertyInfo property, object value);
    }
}
