using AutoFilterer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Types
{
    public class SelectorFilterBase<TResult> : FilterBase, ISelectorFilter<TResult>
        where TResult : class, new()
    {
        public IQueryable<TResult> ApplySelectTo<TEntity>(IQueryable<TEntity> query)
        {
            // TODO: Might be useful: https://github.com/enisn/AutoFilterer/issues/20#issuecomment-751188698
            throw new NotImplementedException();
        }
    }
}
