#if LEGACY_NAMESPACE
using AutoFilterer.Enums;
#endif
using AutoFilterer.Abstractions;
using AutoFilterer.Attributes;
using System.Data;
using System.Linq;

namespace AutoFilterer.Types;

public class OrderableFilterBase : FilterBase, IOrderable
{
    [IgnoreFilter] public virtual Sorting SortBy { get; set; }

    [IgnoreFilter] public virtual string Sort { get; set; }

    public override IQueryable<TEntity> ApplyFilterTo<TEntity>(IQueryable<TEntity> query)
    {
        if (string.IsNullOrEmpty(Sort))
            return base.ApplyFilterTo(query);

        return this.ApplyOrder(base.ApplyFilterTo(query));
    }

    public virtual IOrderedQueryable<TSource> ApplyOrder<TSource>(IQueryable<TSource> source)
        => OrderableBase.ApplyOrder(source, this);

    public virtual IQueryable<TSource> ApplyFilterWithoutOrdering<TSource>(IQueryable<TSource> source)
        => base.ApplyFilterTo(source);
}
