using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Abstractions
{
    public interface IFilter : IFilterableType
    {
        IQueryable<TEntity> ApplyFilterTo<TEntity>(IQueryable<TEntity> query);
    }
}
