using System;

#if LEGACY_NAMESPACE
namespace AutoFilterer.Enums;
#else
namespace AutoFilterer;
#endif

[Flags]
public enum StringFilterOption
{
    Equals = 0,
    StartsWith = 1 << 0,
    EndsWith = 1 << 1,
    Contains = 1 << 2,
}
