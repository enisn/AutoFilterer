using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Extensions
{
    public static class QueryExtensions
    {
        public static IQueryable<T> ToPaged<T>(this IQueryable<T> source, int page, int pageSize)
            => source.Skip((page - 1) * pageSize).Take(pageSize);
    }
}
