using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Enums
{
    /// <summary>
    /// Combinign logic of queries.
    /// 0 - And
    /// 1 - Or
    /// </summary>
    public enum CombineType
    {
        /// <summary>
        /// Places '&amp;&amp;' between comparisons.
        /// </summary>
        And,
        /// <summary>
        /// Places '||' between comparisons.
        /// </summary>
        Or
    }
}
