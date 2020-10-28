using AutoFilterer.Abstractions;
using AutoFilterer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Dynamics
{
    public class DynamicFilter : Dictionary<string, string>, IFilter
    {
        public string Value { get; set; }
        public static implicit operator DynamicFilter(string value) => new DynamicFilter(value);
        public static explicit operator string(DynamicFilter value) => value.Value;

        public CombineType CombineWith { get; set; }

        public bool IsPrimitive => !string.IsNullOrEmpty(Value);

        public DynamicFilter()
        {
        }

        public DynamicFilter(string value)
        {
            this.Value = value;
        }

        public override string ToString() => this.Value;

        public IQueryable<TEntity> ApplyFilterTo<TEntity>(IQueryable<TEntity> query)
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

        public Expression BuildExpression(Type entityType, Expression body)
        {
            Expression finalExpression = body;

            foreach (var key in this.Keys)
            {
                var filter = new DynamicFilter(this[key]);

                if (filter.IsPrimitive)
                {
                    var targetProperty = entityType.GetProperty(filter.Value);

                    var prop = Expression.Property(body, targetProperty.Name);
                    var param = Expression.Constant(Convert.ChangeType((string)filter, targetProperty.PropertyType));

                    var exp = Expression.Equal(prop, param);

                    var combined = Combine(finalExpression, exp);
                    finalExpression = Combine(body, combined);
                }
                else
                {
                    throw new NotImplementedException("Inner objects are not supported yet!");
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
                    return Expression.AndAlso(left, right);
                case CombineType.Or:
                    return Expression.OrElse(left, right);
                default:
                    return right;
            }
        }
    }
}
