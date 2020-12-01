using AutoFilterer.Abstractions;
using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using AutoFilterer.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

                    Expression innerExpression = null;

                    foreach (var targetPropertyName in attribute.PropertyNames)
                    {
                        var targetProperty = entityType.GetProperty(targetPropertyName);
                        if (targetProperty == null)
                            continue;

                        var bodyParameter = finalExpression is MemberExpression ? finalExpression : body;

                        var expression = attribute.BuildExpressionForProperty(bodyParameter, targetProperty, filterProperty, val);
                        innerExpression = innerExpression.Combine(expression, attribute.CombineWith);
                    }

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
