#if LEGACY_NAMESPACE
using AutoFilterer.Enums;
#endif
using AutoFilterer.Abstractions;
using AutoFilterer.Attributes;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoFilterer.Types;

public class OrderableBase : IOrderable
{
    private static readonly MethodInfo orderBy = typeof(Queryable).GetMethods().First(x => x.Name == nameof(Queryable.OrderBy));
    private static readonly MethodInfo orderByDescending = typeof(Queryable).GetMethods().First(x => x.Name == nameof(Queryable.OrderByDescending));

    [IgnoreFilter] public virtual Sorting SortBy { get; set; }
    [IgnoreFilter] public virtual string Sort { get; }

    public virtual IOrderedQueryable<TSource> ApplyOrder<TSource>(IQueryable<TSource> queryable)
    {
        return ApplyOrder(queryable, this);
    }

    public static IOrderedQueryable<TSource> ApplyOrder<TSource>(IQueryable<TSource> source, IOrderable orderable)
    {
        if (string.IsNullOrEmpty(orderable.Sort))
            throw new ArgumentNullException(nameof(orderable.Sort));

        var parameter = Expression.Parameter(typeof(TSource), "o");

        var property = GetMemberExpression(parameter, orderable.Sort);
        var lambda = Expression.Lambda(property, parameter);

        var attribute = orderable.GetType().GetCustomAttribute<PossibleSortingsAttribute>();
        if (attribute != null && !attribute.PropertyNames.Contains(orderable.Sort))
            throw new ArgumentException($"{orderable.Sort} field is not allowed to sort! Check PossibleSortings Attribute at top of filter object.", paramName: "Sort: " + orderable.Sort);

        switch (orderable.SortBy)
        {
            case Sorting.Ascending:
                return orderBy.MakeGenericMethod(typeof(TSource), property.Type).Invoke(null, parameters: new object[] { source, lambda }) as IOrderedQueryable<TSource>;

            case Sorting.Descending:
                return orderByDescending.MakeGenericMethod(typeof(TSource), property.Type).Invoke(null, parameters: new object[] { source, lambda }) as IOrderedQueryable<TSource>;
        }

        throw new InvalidOperationException("Invalid Sorting type in ApplyOrder method");
    }

    // TODO: AutoFilterer.Dynamics feature.
    private static MemberExpression GetMemberExpression(Expression parameter, string name)
    {
        if (!name.Contains("."))
            return Expression.Property(parameter, name);

        var expression = parameter;
        foreach (var item in name.Split('.'))
        {
            expression = GetMemberExpression(expression, item);
        }

        return expression as MemberExpression;
    }
}
