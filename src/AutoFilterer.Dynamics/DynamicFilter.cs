using AutoFilterer.Abstractions;
using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using AutoFilterer.Extensions;
using AutoFilterer.Types;
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

        private static readonly Dictionary<string, IFilterableType> specialKeywords = new Dictionary<string, IFilterableType>
        {
            { "eq", new OperatorComparisonAttribute(OperatorType.Equal) },
            { "not", new OperatorComparisonAttribute(OperatorType.NotEqual) },
            { "gt", new OperatorComparisonAttribute(OperatorType.GreaterThan) },
            { "gte", new OperatorComparisonAttribute(OperatorType.GreaterThanOrEqual) },
            { "lt", new OperatorComparisonAttribute(OperatorType.LessThan) },
            { "lte", new OperatorComparisonAttribute(OperatorType.LessThanOrEqual) },
            { "contains", new StringFilterOptionsAttribute(StringFilterOption.Contains) },
            { "endswith", new StringFilterOptionsAttribute(StringFilterOption.EndsWith) },
            { "startswith", new StringFilterOptionsAttribute(StringFilterOption.StartsWith) },
        };

        public CombineType CombineWith { get; set; }

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
                var filterValue = new DynamicFilter(this[key]);
                if (IsPrimitive(key))
                {
                    var targetProperty = entityType.GetProperty(key);
                    var value = Convert.ChangeType((string)filterValue, targetProperty.PropertyType);
                    var exp = new OperatorComparisonAttribute(OperatorType.Equal).BuildExpression(body, targetProperty, filterProperty: null, value);

                    var combined = finalExpression.Combine(exp, CombineWith);
                    finalExpression = body.Combine(combined, CombineWith);
                }
                else
                {
                    var splitted = key.Split('.');
                    if (IsNotInnerObject(splitted))
                    {
                        var propName = splitted[0];
                        var targetProperty = entityType.GetProperty(propName);
                        var value = Convert.ChangeType((string)filterValue, targetProperty.PropertyType);
                        var comparisonKeyword = splitted[1];
                        if (specialKeywords.TryGetValue(comparisonKeyword, out IFilterableType filterable))
                        {
                            var exp = filterable.BuildExpression(body, targetProperty, filterProperty: null, value);

                            var combined = finalExpression.Combine(exp, CombineWith);
                            finalExpression = body.Combine(combined, CombineWith);
                        }
                    }
                    else
                    {

                        throw new NotImplementedException("Inner objects are not supported yet!");
                    }

                }
            }

            return finalExpression;

            bool IsNotInnerObject(string[] splitted) => splitted.Length == 2;
        }

        private bool IsPrimitive(string key) => !key.Contains('.');
    }
}
