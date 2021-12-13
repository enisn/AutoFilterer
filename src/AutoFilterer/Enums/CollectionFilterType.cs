using System;

#if LEGACY_NAMESPACE
namespace AutoFilterer.Enums;
#else
namespace AutoFilterer;
#endif

[Flags]
public enum CollectionFilterType
{
    Any,
    All,
}
