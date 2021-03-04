using AutoFilterer.Extensions;
using AutoFilterer.Tests.Core;
using AutoFilterer.Tests.Environment.Dtos;
using AutoFilterer.Tests.Environment.Models;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AutoFilterer.Tests.Attributes
{
    public class ToLowerContainsComparisonAttributeTests
    {
        [Theory, AutoMoqData]
        public void BuildExpression_ShouldGenerateQueryCorrect_WithoutAttribute(List<Book> dummyData)
        {
            // Arrange
            var filter = new BookFilter_LowerContains
            {
                Query = "a"
            };
            IQueryable<Book> query = dummyData.AsQueryable();

            // Act
            var filteredQuery = query.ApplyFilter(filter);
            var result = filteredQuery.ToList();
            // Assert
            var actualResult = query.Where(x => x.Title.ToLower().Contains(filter.Query.ToLower())).ToList();

            Assert.Equal(result.Count, actualResult.Count);
        }
    }
}
