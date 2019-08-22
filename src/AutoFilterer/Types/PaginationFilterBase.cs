using AutoFilterer.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Types
{
    public class PaginationFilterBase : FilterBase
    {
        public virtual int Page { get; set; } = 1;
        public virtual int PerPage { get; set; } = 10;

        public override IQueryable<TEntity> ApplyFilterTo<TEntity>(IQueryable<TEntity> query)
        {
            if (query is IOrderedQueryable<TEntity> ordered)
                return this.ApplyFilterTo(ordered);

            return base.ApplyFilterTo(query);
        }

        public virtual IQueryable<TEntity> ApplyFilterTo<TEntity>(IOrderedQueryable<TEntity> query) 
            => base.ApplyFilterTo(query).ToPaged(Page, PerPage);
    }

    public class PaginationFilterBase<T> : FilterBase<T>
    {
        public virtual int Page { get; set; } = 1;
        public virtual int PerPage { get; set; } = 10;

        public virtual IQueryable<T> ApplyFilterTo(IOrderedQueryable<T> query) 
            => base.ApplyFilterTo(query).ToPaged(Page, PerPage);

        public virtual IQueryable<TEntity> ApplyFilterTo<TEntity>(IOrderedQueryable<TEntity> query) 
            => base.ApplyFilterTo(query).ToPaged(Page, PerPage);
    }
}
