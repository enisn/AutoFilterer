using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Abstractions
{
    public interface IFilteringOptions : IFilteringOptions<object>
    {

    }
    public interface IFilteringOptions<TSource>
    {
        Func<TSource, TSource, bool> FilterRuleFunc { get; set; }
    }
}
