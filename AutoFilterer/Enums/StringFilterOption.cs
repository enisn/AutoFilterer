using System;
using System.Collections.Generic;
using System.Text;

namespace AutoFilterer.Enums
{
    [Flags]
    public enum StringFilterOption
    {
        Equals = 0,
        StartsWith = 1 << 0,
        EndsWith = 1 << 1,
        Contains = 1 << 2,
        GreaterThan = 1 << 3,
        LessThan = 1 << 4
    }
}
