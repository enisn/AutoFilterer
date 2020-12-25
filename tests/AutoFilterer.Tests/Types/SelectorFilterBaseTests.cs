using AutoFilterer.Tests.Environment.Dtos;
using AutoFilterer.Tests.Environment.Models;
using AutoFilterer.Tests.Environment.Statics;
using AutoFilterer.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using Xunit;
using System.Linq.Expressions;

namespace AutoFilterer.Tests.Types
{
    public class SelectorFilterBaseTests
    {
        private readonly Expression<Func<Book, BookOutputDto>> defaultSelect = s => new BookOutputDto
        {
            Author = s.Author,
            Id = s.Id,
            IsPublished = s.IsPublished,
            ReadCount = s.ReadCount,
            Title = s.Title,
            TotalPage = s.TotalPage,
            Views = s.Views
        };

        [Theory, AutoMoqData]
        public void ApplySelect_ShouldMapAllProperties_WithEmptyFilter(List<Book> books)
        {
            // Arrange
            var selectorFilter = new BookSelectorFilter();
            var queryable = books.AsQueryable();
            var expected = queryable.Select(defaultSelect).ToList();

            // Act
            var actualQuery = queryable.ApplySelect(selectorFilter);
            var actual = actualQuery.ToList();

            // Assert
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i].Author, actual[i].Author);
                Assert.Equal(expected[i].Id, actual[i].Id);
                Assert.Equal(expected[i].IsPublished, actual[i].IsPublished);
                Assert.Equal(expected[i].ReadCount, actual[i].ReadCount);
                Assert.Equal(expected[i].Title, actual[i].Title);
                Assert.Equal(expected[i].TotalPage, actual[i].TotalPage);
                Assert.Equal(expected[i].Views, actual[i].Views);
            }
        }
        [Theory, AutoMoqData]
        public void ApplyFilter_ShouldMapAllProperties_WithEmptyFilter(List<Book> books)
        {
            // Arrange
            var selectorFilter = new BookSelectorFilter();
            var queryable = books.AsQueryable();
            var expected = queryable.Select(defaultSelect).ToList();

            // Act
            var actualQuery = queryable.ApplyFilter(selectorFilter);
            var actual = actualQuery.ToList();

            // Assert
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i].Author, actual[i].Author);
                Assert.Equal(expected[i].Id, actual[i].Id);
                Assert.Equal(expected[i].IsPublished, actual[i].IsPublished);
                Assert.Equal(expected[i].ReadCount, actual[i].ReadCount);
                Assert.Equal(expected[i].Title, actual[i].Title);
                Assert.Equal(expected[i].TotalPage, actual[i].TotalPage);
                Assert.Equal(expected[i].Views, actual[i].Views);
            }
        }

        [Theory, AutoMoqData(count: 32)]
        public void ApplyFilter_ShouldMapAndFilter_WithTotalPageFilter(List<Book> books)
        {
            // Arrange
            var filter = new BookSelectorFilter_TotalPage { TotalPage = books[2].TotalPage };
            var queryable = books.AsQueryable();
            var expected = queryable.Where(x => x.TotalPage == filter.TotalPage).Select(defaultSelect).ToList();

            // Act
            var actualQuery = queryable.ApplyFilter(filter);
            var actual = actualQuery.ToList();

            // Assert
            Assert.Equal(expected.Count, actual.Count);

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i].Author, actual[i].Author);
                Assert.Equal(expected[i].Id, actual[i].Id);
                Assert.Equal(expected[i].IsPublished, actual[i].IsPublished);
                Assert.Equal(expected[i].ReadCount, actual[i].ReadCount);
                Assert.Equal(expected[i].Title, actual[i].Title);
                Assert.Equal(expected[i].TotalPage, actual[i].TotalPage);
                Assert.Equal(expected[i].Views, actual[i].Views);
            }
        }
    }
}
