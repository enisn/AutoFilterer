using AutoFilterer.Abstractions;
using AutoFilterer.Attributes;
using AutoFilterer.Enums;
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
    public class FilterBase : IFilter
    {
        public CombineType CombineWith { get; set; }

        public virtual IQueryable<TEntity> ApplyFilterTo<TEntity>(IQueryable<TEntity> query)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "x");

            var exp = BuildExpression(typeof(TEntity), parameter);
            if (exp == null)
                return query;

            var lambda = Expression.Lambda<Func<TEntity, bool>>(exp, parameter);
            //var lambda = exp as Expression<Func<TEntity, bool>>;
            return query.Where(lambda);
        }

        public Expression BuildExpression(Type entityType, Expression body)
        {
            Expression finalExpression = null;
            var _type = this.GetType();
            foreach (var entityProperty in entityType.GetProperties())
            {
                try
                {
                    var filterProperty = _type.GetProperty(entityProperty.Name);
                    if (filterProperty == null)
                        continue;

                    var val = filterProperty.GetValue(this);
                    if (val == null)
                        continue;

                    var attribute = filterProperty.GetCustomAttribute<FilteringOptionsBaseAttribute>(false);

                    if (attribute != null)
                    {
                        var expression = attribute.BuildExpression(body, entityProperty, val);
                        finalExpression = Combine(finalExpression, expression);
                        continue;
                    }

                    if (val is IFilter filter)
                    {
                        if (typeof(ICollection).IsAssignableFrom(entityProperty.PropertyType))
                        {
                            var type = entityProperty.PropertyType.GetGenericArguments().FirstOrDefault();
                            
                            var prop = Expression.Property(body, entityProperty.Name);
                            var parameter = Expression.Parameter(type, "a");

                            var methodInfo = typeof(Enumerable).GetMethods().LastOrDefault(x => x.Name == "Any");
                            var method = methodInfo.MakeGenericMethod(type);
                            var query = filter.BuildExpression(type, body: parameter);
                            var lambda = Expression.Lambda(query, parameter);
                            var expression = Expression.Call(
                                                        method: method,
                                                        instance: null,
                                                        arguments: new Expression[] { prop, lambda }
                                );
                                //prop, methodInfo, lambda);
                            finalExpression = Combine(finalExpression, expression);
                            continue;
                        }
                        else
                        {
                            var parameter = Expression.Property(body, entityProperty.Name);
                            var expression = filter.BuildExpression(entityProperty.PropertyType, parameter);
                            finalExpression = Combine(finalExpression, expression);
                            continue;
                        }
                    }

                    if (val is IFilterableType filterableProperty)
                    {
                        var expression = filterableProperty.BuildExpression(body, entityProperty, val);
                        finalExpression = Combine(finalExpression, expression);
                    }
                    else
                    {
                        var comparison = Expression.Equal(
                                            Expression.Property(body, entityProperty.Name),
                                            Expression.Constant(val)
                                            );

                        finalExpression = Combine(finalExpression, comparison);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex?.ToString());
                }
            }

            return finalExpression;
        }

        private Expression Combine(Expression body, Expression extend)
        {
            if (body == null)
                return extend;
            if (extend == null)
                return body;

            if (body is BinaryExpression && extend is BinaryExpression)
                switch (CombineWith)
                {
                    case CombineType.And:
                        return Expression.And(body, extend);
                    case CombineType.Or:
                        return Expression.Or(body, extend);
                    default:
                        return Expression.And(body, extend);
                }

            return extend;
        }

        private Expression<Func<T, bool>> AndOrSelf<T>(Expression baseExp, Expression<Func<T, bool>> newExpression)
        {
            if (baseExp == null)
                return newExpression;
            else
                return Expression.Lambda<Func<T, bool>>(Expression.And(baseExp, newExpression));
        }
    }
}
