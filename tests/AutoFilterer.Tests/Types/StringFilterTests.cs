using AutoFilterer.Extensions;
using AutoFilterer.Tests.Core;
using AutoFilterer.Tests.Environment.Dtos;
using AutoFilterer.Tests.Environment.Models;
using AutoFilterer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AutoFilterer.Tests.Types
{
    public class StringFilterTests
    {
        [Theory, AutoMoqData(count:64)]
        public void BuildExpression_TitleWithContains_ShouldMatchCount(List<Book> data)
        {
            // Arrange
            var filter = new BookFilter_StringFilter_Title
            {
                Title = new StringFilter
                {
                    Contains = "ab"
                }
            };

            // Act
            var query = data.AsQueryable().ApplyFilter(filter);
            var result = query.ToList();

            // Assert
            var actualResult = data.AsQueryable().Where(x => x.Title.Contains(filter.Title.Contains)).ToList();

            Assert.Equal(actualResult.Count, result.Count);
            foreach (var item in actualResult)
                Assert.Contains(item, result);
        }

        [Theory, AutoMoqData(count:64)]
        public void BuildExpression_TitleWithContainsCaseInsensitive_ShouldMatchCount(List<Book> data)
        {
            // Arrange
            var filter = new BookFilter_StringFilter_Title
            {
                Title = new StringFilter
                {
                    Contains = "Ab",
                    Compare = StringComparison.InvariantCultureIgnoreCase
                }
            };

            // Act
            var query = data.AsQueryable().ApplyFilter(filter);
            var result = query.ToList();

            // Assert
            var actualResult = data.AsQueryable().Where(x => x.Title.Contains(filter.Title.Contains, StringComparison.InvariantCultureIgnoreCase)).ToList();

            Assert.Equal(actualResult.Count, result.Count);
            foreach (var item in actualResult)
                Assert.Contains(item, result);
        }
    }
}
