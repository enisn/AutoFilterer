using AutoFilterer.Abstractions;
using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Types
{
    public class OrderableBase : IOrderable
    {
        private static readonly MethodInfo orderBy = typeof(Queryable).GetMethods().First(x => x.Name == nameof(Queryable.OrderBy));
        private static readonly MethodInfo orderByDescending = typeof(Queryable).GetMethods().First(x => x.Name == nameof(Queryable.OrderByDescending));

        [IgnoreFilter] public virtual Sorting SortBy { get; set; }
        [IgnoreFilter] public virtual string Sort { get; }

        public IOrderedQueryable<TSource> ApplyOrder<TSource>(IQueryable<TSource> queryable)
        {
            return ApplyOrder(queryable, this);
        }

        public static IOrderedQueryable<TSource> ApplyOrder<TSource>(IQueryable<TSource> source, IOrderable orderable)
        {
            if (string.IsNullOrEmpty(orderable.Sort))
                throw new ArgumentNullException(nameof(orderable.Sort));

            var parameter = Expression.Parameter(typeof(TSource), "o");
            var property = Expression.Property(parameter, orderable.Sort);
            var lambda = Expression.Lambda(property, parameter);

            var attribute = orderable.GetType().GetCustomAttribute<PossibleSortingsAttribute>();
            if (attribute != null && !attribute.PropertyNames.Contains(orderable.Sort))
                throw new ArgumentException($"{orderable.Sort} field is not allowed to sort! Check PossibleSortings Attribute at top of filter object.", paramName: "Sort: "+orderable.Sort);

            switch (orderable.SortBy)
            {
                case Sorting.Ascending:
                    return orderBy.MakeGenericMethod(typeof(TSource), property.Type).Invoke(null, parameters: new object[] { source, lambda }) as IOrderedQueryable<TSource>;

                case Sorting.Descending:
                    return orderByDescending.MakeGenericMethod(typeof(TSource), property.Type).Invoke(null, parameters: new object[] { source, lambda }) as IOrderedQueryable<TSource>;
            }

            throw new InvalidOperationException("Invalid Sorting type in ApplyOrder method");
        }
    }
}
