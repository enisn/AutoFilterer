#if LEGACY_NAMESPACE
using AutoFilterer.Enums;
#endif
using AutoFilterer.Tests.Environment.Dtos;
using AutoFilterer.Tests.Environment.Models;
using AutoFilterer.Tests.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AutoFilterer.Tests.Types;

public class OrderableBaseTests
{
    [Theory, AutoMoqData(count: 16)]
    public void ApplyFilterTo_WithEmptyParameter_ShouldMatchExpected(List<Book> data)
    {
        // Arrange
        var filter = new BookFilter_Orderable
        {
            Sort = null,
            PerPage = data.Count
        };

        // Act
        var query = filter.ApplyFilterTo(data.AsQueryable());
        var result = query.ToList();

        // Assert
        var expected = data.AsQueryable().ToList();

        Assert.Equal(expected.Count, result.Count);
        for (int i = 0; i < expected.Count; i++)
            Assert.Equal(expected[i], result[i]);
    }

    [Theory, AutoMoqData(count: 16)]
    public void ApplyOrder_WithEmptyParameter_ShouldThrowArgumentNullException(List<Book> data)
    {
        // Arrange
        var filter = new BookFilter_Orderable
        {
            Sort = null,
        };

        // Act
        Action act = () => filter.ApplyOrder(data.AsQueryable());

        // Assert
        Assert.Throws<ArgumentNullException>(nameof(filter.Sort), act);
    }

    [Theory, AutoMoqData(count: 16)]
    public void ApplyOrder_TitleAscending_ShouldMatchExpected(List<Book> data)
    {
        // Arrange
        var filter = new BookFilter_Orderable
        {
            Sort = nameof(Book.Title),
            SortBy = Sorting.Ascending
        };

        // Act
        var query = filter.ApplyOrder(data.AsQueryable());
        var result = query.ToList();

        // Assert
        var expected = data.AsQueryable().OrderBy(o => o.Title).ToList();
        Assert.Equal(expected.Count, result.Count);

        for (int i = 0; i < expected.Count; i++)
            Assert.Equal(expected[i], result[i]);
    }

    [Theory, AutoMoqData(count: 16)]
    public void ApplyOrder_TitleDescending_ShouldMatchExpected(List<Book> data)
    {
        // Arrange
        var filter = new BookFilter_Orderable
        {
            Sort = nameof(Book.Title),
            SortBy = Sorting.Descending
        };

        // Act
        var query = filter.ApplyOrder(data.AsQueryable());
        var result = query.ToList();

        // Assert
        var expected = data.AsQueryable().OrderByDescending(o => o.Title).ToList();
        Assert.Equal(expected.Count, result.Count);

        for (int i = 0; i < expected.Count; i++)
            Assert.Equal(expected[i], result[i]);
    }

    [Theory, AutoMoqData(count: 16)]
    public void ApplyOrder_TotalPageAscending_ShouldMatchExpected(List<Book> data)
    {
        // Arrange
        var filter = new BookFilter_Orderable
        {
            Sort = nameof(Book.TotalPage),
            SortBy = Sorting.Ascending
        };

        // Act
        var query = filter.ApplyOrder(data.AsQueryable());
        var result = query.ToList();

        // Assert
        var expected = data.AsQueryable().OrderBy(o => o.TotalPage).ToList();
        Assert.Equal(expected.Count, result.Count);

        for (int i = 0; i < expected.Count; i++)
            Assert.Equal(expected[i], result[i]);
    }

    [Theory, AutoMoqData(count: 16)]
    public void ApplyOrder_TotalPageDescending_ShouldMatchExpected(List<Book> data)
    {
        // Arrange
        var filter = new BookFilter_Orderable
        {
            Sort = nameof(Book.TotalPage),
            SortBy = Sorting.Ascending
        };

        // Act
        var query = filter.ApplyOrder(data.AsQueryable());
        var result = query.ToList();

        // Assert
        var expected = data.AsQueryable().OrderBy(o => o.TotalPage).ToList();
        Assert.Equal(expected.Count, result.Count);

        for (int i = 0; i < expected.Count; i++)
            Assert.Equal(expected[i], result[i]);
    }

    [Theory, AutoMoqData(count: 16)]
    public void ApplyOrder_WithInnerObject_ShouldMatchExpected(List<User> data)
    {
        // Arrange
        var filter = new BookFilter_Orderable
        {
            Sort = "Preferences.SecurityLevel",
            SortBy = Sorting.Ascending
        };

        // Act
        var query = filter.ApplyOrder(data.AsQueryable());
        var result = query.ToList();

        // Assert
        var expected = data.AsQueryable().OrderBy(o => o.Preferences.SecurityLevel).ToList();
        Assert.Equal(expected.Count, result.Count);

        for (int i = 0; i < expected.Count; i++)
            Assert.Equal(expected[i], result[i]);
    }
}
