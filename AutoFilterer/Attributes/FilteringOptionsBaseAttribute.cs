using AutoFilterer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FilteringOptionsBaseAttribute : Attribute, IFilteringOptions
    {
        Func<object, object, bool> IFilteringOptions<object>.FilterRuleFunc { get; set; }
    }
}
