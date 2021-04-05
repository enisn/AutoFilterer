#if LEGACY_NAMESPACE
using AutoFilterer.Enums;
#endif
using AutoFilterer.Tests.Environment.Dtos;
using AutoFilterer.Tests.Environment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AutoFilterer.Tests.Attributes
{
    public class CompareToAttributeTests
    {
        [Theory, AutoMoqData(count: 64)]
        public void BuildExpression_MultipleFieldWithOr_ShouldMatchCount(List<Book> dummyData)
        {
            // Arrange
            var filter = new BookFilter_MultiplePropertyWithOrDto
            {
                CombineWith = CombineType.Or,
                Query = "12"
            };

            IQueryable<Book> query = dummyData.AsQueryable();

            // Act
            var result = query.ApplyFilter(filter).ToList();

            // Assert
            var actualResult = query.Where(x => x.Title.Contains(filter.Query) || x.Author.Contains(filter.Query)).ToList();

            Assert.Equal(result.Count, actualResult.Count);
        }

        [Theory, AutoMoqData(count: 64)]
        public void BuildExpression_MultipleFieldWithAnd_ShouldMatchCount(List<Book> dummyData)
        {
            // Arrange
            var filter = new BookFilter_MultiplePropertyWithAndDto
            {
                CombineWith = CombineType.Or,
                Query = "a"
            };

            IQueryable<Book> query = dummyData.AsQueryable();

            // Act
            var filteredQuery = query.ApplyFilter(filter);
            Console.WriteLine(filteredQuery);
            var result = filteredQuery.ToList();

            // Assert
            var actualResult = query.Where(x => x.Title.Contains(filter.Query) && x.Author.Contains(filter.Query)).ToList();
            Assert.Equal(result.Count, actualResult.Count);
        }
    }
}
