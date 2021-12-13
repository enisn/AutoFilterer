using AutoFilterer.Abstractions;
using AutoFilterer.Attributes;
using AutoFilterer.Extensions;
using System;
using System.Linq;

namespace AutoFilterer.Types;

public class PaginationFilterBase : OrderableFilterBase, IOrderablePaginationFilter
{
    [IgnoreFilter]
    public virtual int Page { get; set; } = 1;

    [IgnoreFilter]
    public virtual int PerPage { get; set; } = 10;

    public override IQueryable<TEntity> ApplyFilterTo<TEntity>(IQueryable<TEntity> query)
    {
        if (query is IOrderedQueryable<TEntity> ordered)
            return this.ApplyFilterTo(ordered);

        return base.ApplyFilterTo(query).ToPaged(Page, PerPage);
    }

    public virtual IQueryable<TEntity> ApplyFilterTo<TEntity>(IOrderedQueryable<TEntity> query)
        => base.ApplyFilterTo(query).ToPaged(Page, PerPage);

    public virtual IQueryable<T> ApplyFilterWithoutPagination<T>(IQueryable<T> query)
        => base.ApplyFilterTo(query);

    public virtual IQueryable<T> ApplyFilterWithoutPaginationAndOrdering<T>(IQueryable<T> query)
        => base.ApplyFilterWithoutOrdering(query);
}
