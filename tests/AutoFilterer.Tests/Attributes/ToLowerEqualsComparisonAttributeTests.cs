using AutoFilterer.Extensions;
using AutoFilterer.Tests.Core;
using AutoFilterer.Tests.Environment.Dtos;
using AutoFilterer.Tests.Environment.Models;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using AutoFilterer.Attributes;
using AutoFilterer.Types;
using System;

namespace AutoFilterer.Tests.Attributes;

public class ToLowerEqualsComparisonAttributeTests
{
    [Theory, AutoMoqData(count: 16)]
    public void BuildExpression_ShouldGenerateQueryCorrect_WithAttribute(List<Book> dummyData)
    {
        // Arrange
        var filter = new BookFilter_LowerEquals
        {
            Title = "Test Book"
        };
        IQueryable<Book> query = dummyData.AsQueryable();

        // Act
        var filteredQuery = query.ApplyFilter(filter);
        var result = filteredQuery.ToList();
        
        // Assert
        var actualResult = query.Where(x => x.Title.ToLower().Equals(filter.Title.ToLower())).ToList();

        Assert.Equal(result.Count, actualResult.Count);
    }

    [Theory, AutoMoqData(count: 16)]
    public void BuildExpression_ShouldBeCaseInsensitive(List<Book> dummyData)
    {
        // Arrange
        var filter = new BookFilter_LowerEquals
        {
            Title = "TEST BOOK"
        };
        IQueryable<Book> query = dummyData.AsQueryable();

        // Act
        var filteredQuery = query.ApplyFilter(filter);
        var result = filteredQuery.ToList();
        
        // Assert
        var actualResult = query.Where(x => x.Title.ToLower().Equals(filter.Title.ToLower())).ToList();

        Assert.Equal(result.Count, actualResult.Count);
    }

    [Theory, AutoMoqData(count: 16)]
    public void BuildExpression_ShouldHandleEmptyString(List<Book> dummyData)
    {
        // Arrange
        var filter = new BookFilter_LowerEquals
        {
            Title = ""
        };
        IQueryable<Book> query = dummyData.AsQueryable();

        // Act
        var filteredQuery = query.ApplyFilter(filter);
        var result = filteredQuery.ToList();
        
        // Assert
        var actualResult = query.Where(x => x.Title.ToLower().Equals(filter.Title.ToLower())).ToList();

        Assert.Equal(result.Count, actualResult.Count);
    }

    [Theory, AutoMoqData(count: 16)]
    public void BuildExpression_ShouldHandleNullValue(List<Book> dummyData)
    {
        // Arrange
        var filter = new BookFilter_LowerEquals
        {
            Title = null
        };
        IQueryable<Book> query = dummyData.AsQueryable();

        // Act
        var filteredQuery = query.ApplyFilter(filter);
        var result = filteredQuery.ToList();
        
        // Assert
        // When filter value is null, the framework should skip the filter entirely
        // and return all results (no filtering applied)
        Assert.Equal(dummyData.Count, result.Count);
    }

    [Theory, AutoMoqData(count: 16)]
    public void BuildExpression_ShouldHandleSpecialCharacters(List<Book> dummyData)
    {
        // Arrange
        var filter = new BookFilter_LowerEquals
        {
            Title = "Test-Book_123"
        };
        IQueryable<Book> query = dummyData.AsQueryable();

        // Act
        var filteredQuery = query.ApplyFilter(filter);
        var result = filteredQuery.ToList();
        
        // Assert
        var actualResult = query.Where(x => x.Title.ToLower().Equals(filter.Title.ToLower())).ToList();

        Assert.Equal(result.Count, actualResult.Count);
    }

    [Theory, AutoMoqData(count: 16)]
    public void BuildExpression_ShouldHandleWhitespace(List<Book> dummyData)
    {
        // Arrange
        var filter = new BookFilter_LowerEquals
        {
            Title = "  Test Book  "
        };
        IQueryable<Book> query = dummyData.AsQueryable();

        // Act
        var filteredQuery = query.ApplyFilter(filter);
        var result = filteredQuery.ToList();
        
        // Assert
        var actualResult = query.Where(x => x.Title.ToLower().Equals(filter.Title.ToLower())).ToList();

        Assert.Equal(result.Count, actualResult.Count);
    }

    [Theory, AutoMoqData(count: 16)]
    public void BuildExpression_ShouldHandleUnicodeCharacters(List<Book> dummyData)
    {
        // Arrange
        var filter = new BookFilter_LowerEquals
        {
            Title = "Café Book"
        };
        IQueryable<Book> query = dummyData.AsQueryable();

        // Act
        var filteredQuery = query.ApplyFilter(filter);
        var result = filteredQuery.ToList();
        
        // Assert
        var actualResult = query.Where(x => x.Title.ToLower().Equals(filter.Title.ToLower())).ToList();

        Assert.Equal(result.Count, actualResult.Count);
    }

    [Theory, AutoMoqData(count: 16)]
    public void BuildExpression_ShouldReturnEmptyResult_WhenNoMatch(List<Book> dummyData)
    {
        // Arrange
        var filter = new BookFilter_LowerEquals
        {
            Title = "NonExistentBook"
        };
        IQueryable<Book> query = dummyData.AsQueryable();

        // Act
        var filteredQuery = query.ApplyFilter(filter);
        var result = filteredQuery.ToList();
        
        // Assert
        var actualResult = query.Where(x => x.Title.ToLower().Equals(filter.Title.ToLower())).ToList();

        Assert.Equal(result.Count, actualResult.Count);
        Assert.Empty(result);
    }

    [Theory, AutoMoqData(count: 16)]
    public void BuildExpression_ShouldReturnExactMatchesOnly(List<Book> dummyData)
    {
        // Arrange
        var filter = new BookFilter_LowerEquals
        {
            Title = "Test"
        };
        IQueryable<Book> query = dummyData.AsQueryable();

        // Act
        var filteredQuery = query.ApplyFilter(filter);
        var result = filteredQuery.ToList();
        
        // Assert
        var actualResult = query.Where(x => x.Title.ToLower().Equals(filter.Title.ToLower())).ToList();

        Assert.Equal(result.Count, actualResult.Count);
        
        // Verify that all results are exact matches (not partial matches)
        foreach (var book in result)
        {
            Assert.Equal(filter.Title.ToLower(), book.Title.ToLower());
        }
    }
}

public class BookFilter_LowerEquals : FilterBase
{
    [ToLowerEqualsComparison]
    public string Title { get; set; }
} 