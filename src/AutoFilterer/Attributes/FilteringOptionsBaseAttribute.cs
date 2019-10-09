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
        public abstract Expression BuildExpression(Expression expressionBody, PropertyInfo property, object value);
    }
}
