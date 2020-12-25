using AutoFilterer.Abstractions;
using System;
using System.Linq;

namespace AutoFilterer.Extensions
{
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
        /// <returns></returns>
        public static IOrderedQueryable<T> ApplyOrder<T>(this IQueryable<T> source, IOrderable filter)
        {
            return filter.ApplyOrder(source);
        }

        public static IQueryable<TResult> ApplyFilter<T, TResult>(this IQueryable<T> source, ISelectorFilter<TResult> filter)
            where TResult : class, new()
        {
            return filter.ApplySelectTo(filter.ApplyFilterTo(source));
        }

        /// <summary>
        /// Applies only Select to your query.
        /// </summary>
        /// <remarks>
        /// In example: 
        /// <br />
        /// IQueryable&lt;<typeparamref name="T"/>&gt; query = GetQueryable();
        /// <br />
        /// query.Select(new <typeparamref name="TResult"/>{ ... });
        /// </remarks>
        /// <typeparam name="T">Type of your source model. Might be your Entity.</typeparam>
        /// <typeparam name="TResult">Type of to be projected model. Might be your DTO.</typeparam>
        /// <param name="source">Source to be selected.</param>
        /// <param name="filter">Filter object to be referenced how to filter.</param>
        /// <returns>Projected <see cref="IQueryable{T}"/>.</returns>
        public static IQueryable<TResult> ApplySelect<T, TResult>(this IQueryable<T> source, ISelectorFilter<TResult> filter)
            where TResult : class, new()
        {
            return filter.ApplySelectTo(source);
        }
    }
}
