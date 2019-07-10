using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Abstractions
{
    public interface IRange : IFilterableType
    {
        IComparable Min { get; }
        IComparable Max { get; }
    }

    public interface IRange<T> : IFilterableType where T :struct, IComparable
    {
        T? Min { get; set; }
        T? Max { get; set; }
    }
}
