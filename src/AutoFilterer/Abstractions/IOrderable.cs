using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using System.Linq;

namespace AutoFilterer.Abstractions
{
    public interface IOrderable
    {
        [IgnoreFilter] Sorting SortBy { get; set; }

        [IgnoreFilter] string Sort { get; }

        IOrderedQueryable<TSource> ApplyOrder<TSource>(IQueryable<TSource> source);
    }
}
