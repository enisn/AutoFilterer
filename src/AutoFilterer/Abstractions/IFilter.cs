using System;
using System.Linq;
using System.Linq.Expressions;

namespace AutoFilterer.Abstractions;

/// <summary>
/// Base type of AutoFilterer.
/// </summary>
public interface IFilter
{
    Expression BuildExpression(Type entityType, Expression body);
    IQueryable<TEntity> ApplyFilterTo<TEntity>(IQueryable<TEntity> query);
}
