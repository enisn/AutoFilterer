using AutoFilterer.Abstractions;
using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using System.Linq;

namespace AutoFilterer.Types
{
    public class OrderableFilterBase : FilterBase, IOrderable
    {
        [IgnoreFilter] public Sorting SortBy { get; set; }

        [IgnoreFilter] public string Sort { get; set; }

        public override IQueryable<TEntity> ApplyFilterTo<TEntity>(IQueryable<TEntity> query)
        {
            if (string.IsNullOrEmpty(Sort))
                return base.ApplyFilterTo(query);

            return this.ApplyOrder(base.ApplyFilterTo(query));
        }

        public IOrderedQueryable<TSource> ApplyOrder<TSource>(IQueryable<TSource> source) 
            => OrderableBase.ApplyOrder(source, this);

        public IQueryable<TSource> ApplyFilterWithoutOrdering<TSource>(IQueryable<TSource> source)
            => base.ApplyFilterTo(source);
    }
}
