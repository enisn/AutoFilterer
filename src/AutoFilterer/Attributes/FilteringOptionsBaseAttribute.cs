using AutoFilterer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public abstract class FilteringOptionsBaseAttribute : Attribute, IFilterableType
    {
        public abstract Expression<Func<TEntity, bool>> BuildExpression<TEntity>(PropertyInfo property, object value);

        public abstract Expression BuildExpression(PropertyInfo property, object value);
    }
}
