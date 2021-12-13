using System.Linq;

namespace AutoFilterer.Abstractions;

public interface IOrderablePaginationFilter : IPaginationFilter, IOrderable, IFilter
{
    IQueryable<T> ApplyFilterWithoutPaginationAndOrdering<T>(IQueryable<T> query);
}
