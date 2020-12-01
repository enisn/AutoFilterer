using AutoFilterer.Abstractions;
using AutoFilterer.Attributes;
using AutoFilterer.Extensions;
using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoFilterer.Types
{
    /// <summary>
    /// Base class of filter Data Transfer Objects.
    /// </summary>
    public class FilterBase : IFilter
    {
        [IgnoreFilter]
        public virtual CombineType CombineWith { get; set; }

        public virtual IQueryable<TEntity> ApplyFilterTo<TEntity>(IQueryable<TEntity> query)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "x");

            var exp = BuildExpression(typeof(TEntity), parameter);
            if (exp == null)
                return query;

            if (exp is MemberExpression || exp is ParameterExpression)
                return query;

            var lambda = Expression.Lambda<Func<TEntity, bool>>(exp, parameter);
            return query.Where(lambda);
        }

        public virtual Expression BuildExpression(Type entityType, Expression body)
        {
            Expression finalExpression = body;
            var _type = this.GetType();
            foreach (var filterProperty in _type.GetProperties())
            {
                try
                {
                    var val = filterProperty.GetValue(this);
                    if (val == null || filterProperty.GetCustomAttribute<IgnoreFilterAttribute>() != null)
                        continue;

                    var attribute = filterProperty.GetCustomAttribute<CompareToAttribute>(inherit: true) ?? new CompareToAttribute(filterProperty.Name);

                    var bodyParameter = finalExpression is MemberExpression ? finalExpression : body;

                    Expression innerExpression = attribute.BuildExpression(bodyParameter, entityType.GetProperty(attribute.PropertyNames[0]), filterProperty, val);

                    var combined = finalExpression.Combine(innerExpression, CombineWith);
                    finalExpression = combined.Combine(body, CombineWith);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex?.ToString());
                }
            }

            return finalExpression;
        }
    }
}
