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
            => base.ApplyFilterTo(query.ToPaged(Page, PerPage));
    }

    public class PaginationFilterBase<T> : FilterBase<T>
    {
        public virtual int Page { get; set; } = 1;
        public virtual int PerPage { get; set; } = 10;

        public override IQueryable<T> ApplyFilterTo(IQueryable<T> query) 
            => base.ApplyFilterTo(query.ToPaged(Page, PerPage));

        public override IQueryable<TEntity> ApplyFilterTo<TEntity>(IQueryable<TEntity> query) 
            => base.ApplyFilterTo(query.ToPaged(Page, PerPage));
    }
}
