using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Abstractions
{
    /// <summary>
    /// A base interface to compare fields in range.
    /// </summary>
    public interface IRange : IFilterableType
    {
        IComparable Min { get; }
        IComparable Max { get; }
    }

    /// <summary>
    /// A base generic interace to compare fields in range.
    /// </summary>
    /// <typeparam name="T">Type of property.</typeparam>
    public interface IRange<T> : IFilterableType where T :struct, IComparable
    {
        T? Min { get; set; }
        T? Max { get; set; }
    }
}
