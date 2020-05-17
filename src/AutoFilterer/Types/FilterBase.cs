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
            //var lambda = exp as Expression<Func<TEntity, bool>>;
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
                    var entityProperty = entityType.GetProperty(filterProperty.Name);

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
                        innerExpression = Combine(innerExpression, expression, attribute.CombineWith);
                    }

                    //var expression = attribute.BuildExpression(body, entityProperty, filterProperty, val);
                    var combined = Combine(finalExpression, innerExpression);
                    finalExpression = Combine(combined, body);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex?.ToString());
                }
            }

            return finalExpression;
        }

        private protected virtual Expression Combine(Expression body, Expression extend)
        {
            return Combine(body, extend, this.CombineWith);
        }

        private protected virtual Expression Combine(Expression left, Expression right, CombineType combineType)
        {
            if (left == null)
                return right;
            if (right == null)
                return left;

            if (left is ParameterExpression || left is MemberExpression)
                return right;
            if (right is ParameterExpression || right is MemberExpression)
                return left;

            switch (combineType)
            {
                case CombineType.And:
                    return Expression.And(left, right);
                case CombineType.Or:
                    return Expression.Or(left, right);
                default:
                    return right;
            }
        }
    }
}
