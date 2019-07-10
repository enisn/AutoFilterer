using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoFilterer.Abstractions
{
    public interface IFilterableType
    {
        Expression<Func<TEntity, bool>> BuildExpression<TEntity>(PropertyInfo property);
    }
}
