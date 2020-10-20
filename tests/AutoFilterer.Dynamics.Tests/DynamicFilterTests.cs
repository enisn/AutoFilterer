using AutoFilterer.Dynamics.Tests.Environment.Models;
using AutoFilterer.Dynamics.Tests.Environment.Statics;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoFilterer.Extensions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AutoFilterer.Dynamics.Tests
{
    public class DynamicFilterTests
    {
        [Theory, AutoMoqData(count: 128)]
        public void ApplyFilterTo_ShouldFilterCorrect_WithSingleProperty(List<Book> list)
        {
            // Arrange
            DynamicFilter filter = new DynamicFilter { { "TotalPage", "5" } };

            // Act
            var actualQuery = list.AsQueryable().ApplyFilter(filter);
            var exprectedQuery = list.AsQueryable().Where(x => x.TotalPage == 5);

            var actualResult = actualQuery.ToList();
            var expectedResult = exprectedQuery.ToList();

            // Assert
            Assert.Equal(expectedResult.Count, actualResult.Count);

            Assert.Equal(exprectedQuery.ToString(), actualQuery.ToString());

            foreach (var item in expectedResult)
                Assert.Contains(item, actualResult);
        }

        [Theory, AutoMoqData(count: 1024)]
        public void ApplyFilterTo_ShouldFilterCorrect_WithMultiplePropertyCombiningAnd(List<Book> list)
        {
            // Arrange
            DynamicFilter filter = new DynamicFilter { { "TotalPage", "5" },  { "IsPublished", "True" } };
            filter.CombineWith = Enums.CombineType.And;

            // Act
            var actualQuery = list.AsQueryable().ApplyFilter(filter);
            var exprectedQuery = list.AsQueryable().Where(x => x.TotalPage == 5 && x.IsPublished == true);

            var actualResult = actualQuery.ToList();
            var expectedResult = exprectedQuery.ToList();

            // Assert
            Assert.Equal(expectedResult.Count, actualResult.Count);

            foreach (var item in expectedResult)
                Assert.Contains(item, actualResult);

            Assert.Equal(exprectedQuery.ToString(), actualQuery.ToString());
        }

        [Theory, AutoMoqData(count: 32)]
        public void ApplyFilterTo_ShouldFilterCorrect_WithMultiplePropertyCombiningOr(List<Book> list)
        {
            // Arrange
            DynamicFilter filter = new DynamicFilter { { "TotalPage", "5" },  { "IsPublished", "True" } };
            filter.CombineWith = Enums.CombineType.Or;

            // Act
            var actualQuery = list.AsQueryable().ApplyFilter(filter);
            var exprectedQuery = list.AsQueryable().Where(x => x.TotalPage == 5 || x.IsPublished == true);

            var actualResult = actualQuery.ToList();
            var expectedResult = exprectedQuery.ToList();

            // Assert
            Assert.Equal(exprectedQuery.Expression.ToString(), actualQuery.Expression.ToString());
            Assert.Equal(expectedResult.Count, actualResult.Count);

            foreach (var item in expectedResult)
                Assert.Contains(item, actualResult);
        }
    }
}
