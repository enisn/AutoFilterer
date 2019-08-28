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

            return (source as IOrderedQueryable<T>).Skip((page - 1) * pageSize).Take(pageSize);
        }

        public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> source, IFilter filter)
        {
            return filter.ApplyFilterTo(source);
        }

        public static IQueryable<T> ApplyFilter<T>(this IOrderedQueryable<T> source, IFilter filter)
        {
            return filter.ApplyFilterTo(source);
        }
    } 
}
