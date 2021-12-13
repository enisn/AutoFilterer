#if LEGACY_NAMESPACE
using AutoFilterer.Enums;
#endif
using System.Linq.Expressions;

namespace AutoFilterer.Extensions;

public static class ExpressionExtensions
{
    public static Expression Combine(this Expression left, Expression right, CombineType combineType)
    {
        if (left == null)
            return right;
        if (right == null)
            return left;

        if (left is ParameterExpression || left is MemberExpression)
            return right;
        if (right is ParameterExpression || right is MemberExpression)
            return left;

        switch (combineType)
        {
            case CombineType.And:
                return Expression.AndAlso(left, right);
            case CombineType.Or:
                return Expression.OrElse(left, right);
            default:
                return right;
        }
    }
}
