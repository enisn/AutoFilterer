using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Types
{
    /// <summary>
    /// Base class of filter Data Transfer Objects (DTO).
    /// If you don't have access to your Entity models from dto (if they're in seperated libraries), just inherit non-generic type of FilterBase which is <see cref="FilterBase"/>.
    /// </summary>
    /// <typeparam name="TEntity">Entity Type.</typeparam>
    public class FilterBase<TEntity> : FilterBase
    {
        /// <summary>
        /// Automaticly Applies Where() to IQuaryable collection. Please visit project site for more information and customizations.
        /// </summary>
        /// <param name="query"></param>
        public virtual IQueryable<TEntity> ApplyFilterTo(IQueryable<TEntity> query)
        {
            return ApplyFilterTo<TEntity>(query);
        }
    }
}
