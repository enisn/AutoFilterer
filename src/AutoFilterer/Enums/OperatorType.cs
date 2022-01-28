#if LEGACY_NAMESPACE
namespace AutoFilterer.Enums;
#else
namespace AutoFilterer;
#endif

public enum OperatorType
{
    Equal,
    NotEqual,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
    IsNull,
    IsNotNull,
}