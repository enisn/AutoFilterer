using AutoFilterer.Attributes;
using System.Linq;

namespace AutoFilterer.Abstractions;

public interface IPaginationFilter : IFilter
{
    [IgnoreFilter] int Page { get; set; }
    [IgnoreFilter] int PerPage { get; set; }

    IQueryable<T> ApplyFilterWithoutPagination<T>(IQueryable<T> query);
}
