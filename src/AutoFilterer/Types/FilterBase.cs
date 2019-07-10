using AutoFilterer.Abstractions;
using AutoFilterer.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Types
{
    public class FilterBase<TEntity> : IFilterableType
    {
        public IQueryable<TEntity> ApplyFilterTo(IQueryable<TEntity> query)
        {
            var _type = this.GetType();
            foreach (var entityProperty in typeof(TEntity).GetProperties())
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
                        query = query.Where(attribute.BuildExpression<TEntity>(entityProperty, val));
                        continue;
                    }

                    if (val is IFilterableType filterableProperty)
                    {
                        var expression = filterableProperty.BuildExpression<TEntity>(entityProperty, val);
                        query = query.Where(expression);
                    }
                    else
                    {
                        var expression = BuildExpression<TEntity>(entityProperty, val);
                        query = query.Where(expression);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex?.ToString());
                }
            }
            return query;
        }

        public Expression<Func<TModel, bool>> BuildExpression<TModel>(PropertyInfo property, object value)
        {
            var parameter = Expression.Parameter(property.DeclaringType, property.Name);

            var comparison = Expression.Equal(
                                Expression.Property(parameter, property.Name),
                                Expression.Constant(value)
                                );

            return Expression.Lambda<Func<TModel, bool>>(comparison, parameter);
        }
    }

}
