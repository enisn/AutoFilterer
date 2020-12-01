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
    public class CompareToAsAttribute : CompareToAttribute
    {
        private Type filterAttributeType;

        public CompareToAsAttribute(Type filterAttributeType, params string[] propertyNames) : base(propertyNames)
        {
            FilterAttributeType = filterAttributeType;
        }

        public CompareToAsAttribute(Type filterAttributeType, CombineType combineWith, params string[] propertyNames) : base(combineWith, propertyNames)
        {
            FilterAttributeType = filterAttributeType;
        }

        public Type FilterAttributeType { get => filterAttributeType; set => SetFilterAttributeType(value); }
        public IPropertyExpressionable Comparer { get; private set; }

        private void SetFilterAttributeType(Type type)
        {
            if (!typeof(IPropertyExpressionable).IsAssignableFrom(type))
                throw new ArgumentException();

            this.Comparer = (IPropertyExpressionable)Activator.CreateInstance(type);
            filterAttributeType = type;
        }

        public override Expression BuildExpression(Expression expressionBody, PropertyInfo targetProperty, PropertyInfo filterProperty, object value)
        {
            return Comparer.BuildExpression(expressionBody, targetProperty, filterProperty, value);
        }
    }
}
