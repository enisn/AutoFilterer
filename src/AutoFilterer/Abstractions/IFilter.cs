using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Abstractions
{
    public interface IFilter
    {
        Expression BuildExpression(Type entityType, Expression body);
        IQueryable<TEntity> ApplyFilterTo<TEntity>(IQueryable<TEntity> query);
    }
}
