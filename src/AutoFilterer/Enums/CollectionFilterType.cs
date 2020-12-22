using System;

#if LEGACY_NAMESPACE
namespace AutoFilterer.Enums
#else
namespace AutoFilterer
#endif
{
    namespace AutoFilterer
    {
        [Flags]
        public enum CollectionFilterType
        {
            Any,
            All,
        }
    }
}
