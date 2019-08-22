using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Abstractions
{
    /// <summary>
    /// This is a filter object interface. Not a single property, this should contain filtering properties.
    /// </summary>
    public interface IFilter : IFilterableType
    {
        /// <summary>
        /// Main Library objects will call this method. Basicly generates an Linq Expression for a property with given parameters from property info.
        /// </summary>
        /// <param name="entityType">Type of given object.</param>
        /// <param name="filter">Filter object. It's this by default. This is for inner objects.</param>
        /// <returns>Built Lambda Expression for LINQ.</returns>
        Expression BuildExpression(Type entityType, object filter = null);

        Expression<Func<TEntity, bool>> BuildExpression<TEntity>(object filter = null);

        IQueryable<TEntity> ApplyFilterTo<TEntity>(IQueryable<TEntity> query);
    }
}
