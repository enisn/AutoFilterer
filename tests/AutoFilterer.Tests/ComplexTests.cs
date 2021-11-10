using AutoFilterer.Abstractions;
using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using AutoFilterer.Extensions;
using AutoFilterer.Tests.Core;
using AutoFilterer.Types;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace AutoFilterer.Tests
{
    public class ComplexTests
    {
        #region Entities
        public class Product
        {
            public string Name { get; set; }
            public ICollection<ProductOptionValue> ProductOptionValues { get; set; }
        }

        public class ProductOptionValue
        {
            public OptionValue OptionValue { get; set; }
        }

        public class OptionValue
        {
            public string Value { get; set; }
        }
        #endregion

        // ---------------------------------------------
        #region Filters


        public class ProductFilter : OrderableFilterBase
        {
            public string Name { get; set; }
            //...
            public ProductOptionValueFilter ProductOptionValues { get; set; }
        }

        public class ProductOptionValueFilter : FilterBase
        {
            [CollectionFilter(CollectionFilterType.All)]
            public OptionValueFilter[] OptionValue { get; set; }
        }

        public class OptionValueFilter : FilterBase
        {
            public string Value { get; set; }
        }
        #endregion
        // ---------------------------------------------

        public class Person
        {
            public string Name { get; set; }
            public string[] MyProperty { get; set; }
        }
        [Theory, AutoMoqData(count: 64)]
        public void PrimitiveToCollectionTest()
        {

        }

        [Theory, AutoMoqData(count: 64)]
        public void Test(List<Product> products)
        {
            // Arrange
            FilterBase.IgnoreExceptions = false;

            products[0].ProductOptionValues.Add(new ProductOptionValue { OptionValue = new OptionValue { Value = "A" } });
            products[0].ProductOptionValues.Add(new ProductOptionValue { OptionValue = new OptionValue { Value = "A" } });
            products[0].ProductOptionValues.Add(new ProductOptionValue { OptionValue = new OptionValue { Value = "B" } });
            products[0].ProductOptionValues.Add(new ProductOptionValue { OptionValue = new OptionValue { Value = "C" } });

            products[1].ProductOptionValues.Add(new ProductOptionValue { OptionValue = new OptionValue { Value = "A" } });

            var queryable = products.AsQueryable();

            var filter = new ProductFilter
            {
                ProductOptionValues = new ProductOptionValueFilter
                {
                    OptionValue = new []
                    {
                        new OptionValueFilter { Value = "A" },
                        new OptionValueFilter { Value = "B" },
                        new OptionValueFilter { Value = "C" },
                    }
                }
            };

            // Act
            var actual = queryable.ApplyFilter(filter);
            var actualResult = actual.ToList();

            //var expected = queryable.Where(x => x.ProductOptionValues.Any(a => filter.ProductOptionValues.OptionValue.Value.Contains(a.OptionValue.Value)));
            //var expectedResult = expected.ToList();

            // Assert
            Assert.Equal(1, actualResult.Count);
            //foreach (var expectedItem in expectedResult)
            //{
            //    Assert.Contains(expectedItem, actualResult);
            //}
        }

        public class ArrayToArraySearchAttribute : FilteringOptionsBaseAttribute
        {
            public override Expression BuildExpression(Expression expressionBody, PropertyInfo targetProperty, PropertyInfo filterProperty, object value)
            {
                if (value is IFilter filter)
                {
                    // TODO: Make default Comparison here...
                }
                else
                {
                    //var type = targetProperty.PropertyType.GetGenericArguments().FirstOrDefault();
                    //var parameter = Expression.Parameter(type, "a");
                    //var innerLambda = Expression.Lambda(filter.BuildExpression(type, body: parameter), parameter);
                    //var prop = Expression.Property(expressionBody, targetProperty.Name);
                    //var methodInfo = typeof(Enumerable).GetMethods().LastOrDefault(x => x.Name == FilterOption.ToString());
                    //var method = methodInfo.MakeGenericMethod(type);

                    //expressionBody = Expression.Call(
                    //                            method: method,
                    //                            instance: null,
                    //                            arguments: new Expression[] { prop, innerLambda }
                    //    );
                }
                return expressionBody;
            }
        }
    }
}
