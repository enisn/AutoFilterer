using AutoFilterer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Types
{
    public class SelectorFilterBase<TResult> : ISelectorFilter<TResult>
        where TResult : class, new()
    {
        public IQueryable<TEntity> ApplyFilterTo<TEntity>(IQueryable<TEntity> query)
        {
            return query;
        }

        public IQueryable<TResult> ApplySelectTo<TEntity>(IQueryable<TEntity> query)
        {
            throw new NotImplementedException();
        }

        public Expression BuildExpression(Type entityType, Expression body)
        {
            throw new NotImplementedException();
        }
    }
}
