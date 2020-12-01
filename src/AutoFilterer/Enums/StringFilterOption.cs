using System;

namespace AutoFilterer
{
    [Flags]
    public enum StringFilterOption
    {
        Equals = 0,
        StartsWith = 1 << 0,
        EndsWith = 1 << 1,
        Contains = 1 << 2,
    }
}
