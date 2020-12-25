using AutoFilterer.Abstractions;
using System;
using System.Linq;

namespace AutoFilterer.Types
{
    public class SelectorPaginationFilterBase<TResult> : PaginationFilterBase, ISelectorFilter<TResult>
        where TResult : class, new()
    {
        public IQueryable<TResult> ApplySelectTo<TEntity>(IQueryable<TEntity> query)
        {
            // TODO: This might help: https://github.com/enisn/AutoFilterer/issues/20#issuecomment-751188698
            throw new NotImplementedException();
        }
    }
}
