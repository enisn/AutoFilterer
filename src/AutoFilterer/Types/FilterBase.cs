using AutoFilterer.Abstractions;
using AutoFilterer.Attributes;
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
        /// <summary>
        /// Automaticly Applies Where() to IQuaryable collection. Please visit project site for more information and customizations.
        /// </summary>
        /// <param name="query"></param>
        public virtual IQueryable<TEntity> ApplyFilterTo<TEntity>(IQueryable<TEntity> query)
        {
            return query.Where(this.BuildExpression<TEntity>());
        }

        /// <summary>
        /// Generates a LINQ Expression with properties
        /// </summary>
        /// <typeparam name="TModel">Expression to apply model</typeparam>
        /// <param name="property">Reflection <see cref="PropertyInfo"/> to filter</param>
        /// <param name="value">value to filter</param>
        public virtual Expression BuildExpression(Type entityType, object filter = null)
        {
            if (filter == null)
                filter = this;

            Expression lastExpression = null;
            var parameters = new List<ParameterExpression>();
            foreach (var filterProperty in filter.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var val = filterProperty.GetValue(filter);
                if (val == null)
                    continue;

                var entityProperty = entityType.GetProperty(filterProperty.Name, BindingFlags.Instance | BindingFlags.Public);
                if (entityProperty == null)
                    continue;


                Expression currentExpression = null;
                var parameter = Expression.Parameter(entityProperty.DeclaringType, entityProperty.Name);
                parameters.Add(parameter);

                if (typeof(ICollection).IsAssignableFrom(entityProperty.PropertyType) || typeof(IQueryable).IsAssignableFrom(entityProperty.PropertyType))
                {
                    var method =
                        typeof(Queryable).IsAssignableFrom(entityProperty.PropertyType) ?
                        typeof(Queryable).GetMethods().FirstOrDefault(x => x.Name == nameof(Queryable.Any) && x.GetParameters().Length == 2) :
                        typeof(Enumerable).GetMethods().FirstOrDefault(x => x.Name == nameof(Queryable.Any) && x.GetParameters().Length == 2)
                        ;

                    method = method.MakeGenericMethod(entityProperty.PropertyType.GenericTypeArguments[0]);

                    currentExpression = Expression.Equal(
                            Expression.Call(
                                method: method,
                                instance: null,
                                arguments: new[] { Expression.Property(parameter, entityProperty.Name), BuildExpression(entityProperty.PropertyType.GenericTypeArguments[0], val) }
                                ),
                            Expression.Constant(true)
                        );

                }

                if (val is IFilter)
                {
                    currentExpression = BuildExpression(entityProperty, val);
                }
                else if (val is IFilterableType filterable)
                {
                    currentExpression = typeof(IFilterableType).GetMethod(nameof(IFilterableType.BuildExpression)).MakeGenericMethod(entityProperty.DeclaringType).Invoke(filterable, parameters: new[] { entityProperty, val }) as Expression;
                }
                else
                {
                    currentExpression = Expression.Equal(
                                        Expression.Property(parameter, entityProperty.Name),
                                        Expression.Constant(val)
                                        );
                }

                if (lastExpression == null)
                    lastExpression = currentExpression;
                else
                    lastExpression = Expression.And(lastExpression, currentExpression);
            }

            return LambdaExpression.Lambda(lastExpression, parameters);
        }

        public Expression<Func<TEntity, bool>> BildExpression<TEntity>(object filter = null)
        {
            var lastQuery = BuildExpression(typeof(TEntity), filter);
            return (Expression<Func<TEntity, bool>>) lastQuery;
        }

        public Expression BuildExpression(PropertyInfo property, object value)
        {
            return BuildExpression(property.PropertyType, value);
        }

        public Expression<Func<TEntity, bool>> BuildExpression<TEntity>(PropertyInfo property, object value)
        {
            return (Expression<Func<TEntity, bool>>)BuildExpression(property, value);
        }
    }
}
