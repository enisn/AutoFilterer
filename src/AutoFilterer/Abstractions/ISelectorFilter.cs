using System.Linq;

namespace AutoFilterer.Abstractions
{
    public interface ISelectorFilter<TResult> : IFilter
        where TResult : class, new()
    {
        IQueryable<TResult> ApplySelectTo<TEntity>(IQueryable<TEntity> query);
    }
}
