using AutoFilterer.Abstractions;
using System;
using System.Linq;

namespace AutoFilterer.Extensions;

public static class QueryExtensions
{
    public static IQueryable<T> ToPaged<T>(this IOrderedQueryable<T> source, int page, int pageSize)
    {
        if (page <= 0)
            throw new ArgumentOutOfRangeException(nameof(page), "The given parameter can not be zero or negative.");

        if (pageSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "The given parameter can not be zero or negative.");

        return source.Skip((page - 1) * pageSize).Take(pageSize);
    }

    public static IQueryable<T> ToPaged<T>(this IQueryable<T> source, int page, int pageSize)
    {
        if (page <= 0)
            throw new ArgumentOutOfRangeException(nameof(page), "The given parameter can not be zero or negative.");

        if (pageSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "The given parameter can not be zero or negative.");

        return source.Skip((page - 1) * pageSize).Take(pageSize);
    }

    /// <summary>
    /// Applies all filters from <see cref="IFilter"/> type. If your filter object implements Pagination and Ordering this method will apply OrderBy() and Skip().Take() automatically.
    /// <list type="bullet">
    /// You can chain with other LINQ methods with ApplyFilter like:
    /// </list>
    /// <code>
    ///     db.Books
    ///         .Where(x => !x.IsDeleted)
    ///         .ApplyFilter(filter)
    ///         .Select(s => s.Name)
    ///         .ToList();
    /// </code>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="filter">Be careful!
    /// <list type="bullet">
    /// <item>
    /// If your filter implements <see cref="IOrderable"/>, this method also adds <term>OrderBy()</term> or <term>OrderByDescending()</term> method into your query.
    /// </item>
    /// <item>
    /// If your filter implements <see cref="IPaginationFilter"/>, this method also adds <term>Skip().Take()</term>  methods into your query.
    /// </item>
    /// </list>
    /// </param>
    /// </summary>
    /// <returns>Built and queried <see cref="IQueryable"/> result.</returns>
    public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> source, IFilter filter)
    {
        return filter.ApplyFilterTo(source);
    }

    /// <summary>
    /// Applies all filters from <see cref="IFilter"/> type. If your filter object implements Pagination and Ordering this method will apply OrderBy() and Skip().Take() automatically.
    /// <list type="bullet">
    /// You can chain with other LINQ methods with ApplyFilter like:
    /// </list>
    /// <code>
    ///     db.Books
    ///         .Where(x => !x.IsDeleted)
    ///         .ApplyFilter(filter)
    ///         .Select(s => s.Name)
    ///         .ToList();
    /// </code>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="filter">Be careful!
    /// <list type="bullet">
    /// <item>
    /// If your filter implements <see cref="IOrderable"/>, this method also adds <term>OrderBy()</term> or <term>OrderByDescending()</term> method into your query.
    /// </item>
    /// <item>
    /// If your filter implements <see cref="IPaginationFilter"/>, this method also adds <term>Skip().Take()</term>  methods into your query.
    /// </item>
    /// </list>
    /// </param>
    /// </summary>
    /// <returns>Built and queried <see cref="IQueryable"/> result.</returns>
    public static IQueryable<T> ApplyFilter<T>(this IOrderedQueryable<T> source, IFilter filter)
    {
        return filter.ApplyFilterTo(source);
    }

    /// <summary>
    /// Applies only Ordering into your query.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="filter">Only <see cref="IOrderable"/> implemented objects are availabe.</param>
    /// <returns>Final queryable.</returns>
    public static IOrderedQueryable<T> ApplyOrder<T>(this IQueryable<T> source, IOrderable filter)
    {
        return filter.ApplyOrder(source);
    }

    /// <summary>
    /// Applies filter without Skip().Take() methods. It uses only Where() method over your <see cref="IQueryable"/>.
    /// <list type="bullet">
    /// You can use this method to get total count of applied query:
    /// </list>
    /// <code>
    /// var books = db.Books.ApplyFilter(filter).ToList();
    /// var count = db.Books.ApplyFilterWithoutPagination(filter).Count();
    /// </code>
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="filter">Only <see cref="IPaginationFilter"/> implemented objects are available.</param>
    /// <returns>Final queryable.</returns>
    public static IQueryable<TSource> ApplyFilterWithoutPagination<TSource>(this IQueryable<TSource> source, IPaginationFilter filter)
    {
        return filter.ApplyFilterWithoutPagination(source);
    }
}
