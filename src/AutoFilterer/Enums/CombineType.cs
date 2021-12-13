#if LEGACY_NAMESPACE
namespace AutoFilterer.Enums;
#else
namespace AutoFilterer;
#endif

/// <summary>
/// Combining logic of queries.
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
