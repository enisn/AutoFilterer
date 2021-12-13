#if LEGACY_NAMESPACE
using AutoFilterer.Enums;
#endif
using AutoFilterer.Attributes;
using System.Linq;

namespace AutoFilterer.Abstractions;

public interface IOrderable
{
    [IgnoreFilter] Sorting SortBy { get; set; }

    [IgnoreFilter] string Sort { get; }

    IOrderedQueryable<TSource> ApplyOrder<TSource>(IQueryable<TSource> source);
}
