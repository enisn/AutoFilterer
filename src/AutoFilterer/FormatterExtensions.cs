using System.Linq;

namespace AutoFilterer;

public static class FormatterExtensions
{
    public static IQueryable<string> Select<T>(this IQueryable<T> queryable, Formatter<T> formatter) =>
        queryable.Select<T, string>(formatter);
}