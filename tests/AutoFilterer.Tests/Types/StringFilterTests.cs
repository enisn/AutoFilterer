using AutoFilterer.Extensions;
using AutoFilterer.Tests.Core;
using AutoFilterer.Tests.Environment.Dtos;
using AutoFilterer.Tests.Environment.Models;
using AutoFilterer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AutoFilterer.Tests.Types;

public class StringFilterTests
{
    [Theory, AutoMoqData(count: 64)]
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

    [Theory, AutoMoqData(count: 64)]
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
    
    [Theory, AutoMoqData(count: 64)]
    public void BuildExpression_TitleWithNotContains_ShouldMatchCount(List<Book> data)
    {
        // Arrange
        var filter = new BookFilter_StringFilter_Title
        {
            Title = new StringFilter
            {
                NotContains = "ab"
            }
        };

        // Act
        var query = data.AsQueryable().ApplyFilter(filter);
        var result = query.ToList();

        // Assert
        var actualResult = data.AsQueryable().Where(x => !x.Title.Contains(filter.Title.NotContains)).ToList();

        Assert.Equal(actualResult.Count, result.Count);
        foreach (var item in actualResult)
            Assert.Contains(item, result);
    }
    
    [Theory, AutoMoqData(count: 64)]
    public void BuildExpression_TitleWithNotContainsCaseInsensitive_ShouldMatchCount(List<Book> data)
    {
        // Arrange
        var filter = new BookFilter_StringFilter_Title
        {
            Title = new StringFilter
            {
                NotContains = "Ab",
                Compare = StringComparison.InvariantCultureIgnoreCase
            }
        };

        // Act
        var query = data.AsQueryable().ApplyFilter(filter);
        var result = query.ToList();

        // Assert
        var actualResult = data.AsQueryable().Where(x => !x.Title.Contains(filter.Title.NotContains, StringComparison.InvariantCultureIgnoreCase)).ToList();

        Assert.Equal(actualResult.Count, result.Count);
        foreach (var item in actualResult)
            Assert.Contains(item, result);
    }
    
    [Theory, AutoMoqData(count: 64)]
    public void BuildExpression_TitleWithIsNull_ShouldMatchCount(List<Book> data)
    {
        for (var i = 0; i < 5; i++)
        {
            data[i].Title = null;
        }
        
        // Arrange
        var filter = new BookFilter_StringFilter_Title
        {
            Title = new StringFilter
            {
                IsNull = true
            }
        };

        // Act
        var query = data.AsQueryable().ApplyFilter(filter);
        var result = query.ToList();

        // Assert
        var actualResult = data.AsQueryable().Where(x => x.Title == null).ToList();

        Assert.Equal(actualResult.Count, result.Count);
        Assert.Equal(5, actualResult.Count);
        Assert.Equal(5, result.Count);
        foreach (var item in actualResult)
            Assert.Contains(item, result);
    }
    
    [Theory, AutoMoqData(count: 64)]
    public void BuildExpression_TitleWithIsNotNull_ShouldMatchCount(List<Book> data)
    {
        for (var i = 0; i < 5; i++)
        {
            data[i].Title = null;
        }
        
        // Arrange
        var filter = new BookFilter_StringFilter_Title
        {
            Title = new StringFilter
            {
                IsNotNull = true
            }
        };

        // Act
        var query = data.AsQueryable().ApplyFilter(filter);
        var result = query.ToList();

        // Assert
        var actualResult = data.AsQueryable().Where(x => x.Title != null).ToList();

        Assert.Equal(actualResult.Count, result.Count);
        Assert.Equal(64 - 5, actualResult.Count);
        Assert.Equal(64 - 5, result.Count);
        foreach (var item in actualResult)
            Assert.Contains(item, result);
    }
    
    [Theory, AutoMoqData(count: 64)]
    public void BuildExpression_TitleWithIsEmpty_ShouldMatchCount(List<Book> data)
    {
        for (var i = 0; i < 5; i++)
        {
            data[i].Title = "";
        }
        
        // Arrange
        var filter = new BookFilter_StringFilter_Title
        {
            Title = new StringFilter
            {
                IsEmpty = true
            }
        };

        // Act
        var query = data.AsQueryable().ApplyFilter(filter);
        var result = query.ToList();

        // Assert
        var actualResult = data.AsQueryable().Where(x => x.Title == "").ToList();

        Assert.Equal(actualResult.Count, result.Count);
        Assert.Equal(5, actualResult.Count);
        Assert.Equal(5, result.Count);
        foreach (var item in actualResult)
            Assert.Contains(item, result);
    }
    
    [Theory, AutoMoqData(count: 64)]
    public void BuildExpression_TitleWithIsNotEmpty_ShouldMatchCount(List<Book> data)
    {
        for (var i = 0; i < 5; i++)
        {
            data[i].Title = "";
        }
        
        // Arrange
        var filter = new BookFilter_StringFilter_Title
        {
            Title = new StringFilter
            {
                IsNotEmpty = true
            }
        };

        // Act
        var query = data.AsQueryable().ApplyFilter(filter);
        var result = query.ToList();

        // Assert
        var actualResult = data.AsQueryable().Where(x => x.Title != "").ToList();

        Assert.Equal(actualResult.Count, result.Count);
        Assert.Equal(64 - 5, actualResult.Count);
        Assert.Equal(64 - 5, result.Count);
        foreach (var item in actualResult)
            Assert.Contains(item, result);
    }
}
