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

public class RangeTests
{
    private static readonly Random random = new Random();
    [Theory, AutoMoqData(count: 64)]
    public void BuildExpression_Min_ShouldMatchCount(List<Book> dummyData, int min)
    {
        // Arrange
        var filter = new BookFilter_Range_TotalPage
        {
            TotalPage = new Range<int>(min: min, null)
        };

        var query = dummyData.AsQueryable();

        // Act
        var actualQuery = query.ApplyFilter(filter);
        var expectedQuery = query.Where(x => x.TotalPage >= min);

        // Assert
        Assert.Equal(expectedQuery.Count(), actualQuery.Count());
    }

    [Theory, AutoMoqData(count: 64)]
    public void BuildExpression_Max_ShouldMatchCount(List<Book> dummyData, int max)
    {
        // Arrange
        var filter = new BookFilter_Range_TotalPage
        {
            TotalPage = new Range<int>(null, max: max)
        };

        var query = dummyData.AsQueryable();

        // Act
        var actualQuery = query.ApplyFilter(filter);
        var expectedQuery = query.Where(x => x.TotalPage <= max);

        // Assert
        Assert.Equal(expectedQuery.Count(), actualQuery.Count());
    }

    [Theory, AutoMoqData(count: 64)]
    public void BuildExpression_MinMax_ShouldMatchCount(List<Book> dummyData, int min)
    {
        // Arrange
        var max = min + random.Next(0, 20);

        var filter = new BookFilter_Range_TotalPage
        {
            TotalPage = new Range<int>(min, max)
        };

        var query = dummyData.AsQueryable();

        // Act
        var actualQuery = query.ApplyFilter(filter);
        var expectedQuery = query.Where(x => min <= x.TotalPage && x.TotalPage <= max);

        // Assert
        Assert.Equal(expectedQuery.Count(), actualQuery.Count());
    }

    [Theory, AutoMoqData(count: 64)]
    public void BuildExpression_MinWithNullable_ShouldMatchCount(List<Book> dummyData, int min)
    {
        // Arrange
        var filter = new BookFilter_Range_Views
        {
            Views = new Range<int>(min, null)
        };

        var query = dummyData.AsQueryable();

        // Act
        var actualQuery = query.ApplyFilter(filter);
        var expectedQuery = query.Where(x => x.Views.Value >= min);

        // Assert
        Assert.Equal(expectedQuery.Count(), actualQuery.Count());
    }

    [Theory, AutoMoqData(count: 64)]
    public void BuildExpression_MaxWithNullable_ShouldMatchCount(List<Book> dummyData, int max)
    {
        // Arrange
        var filter = new BookFilter_Range_Views
        {
            Views = new Range<int>(null, max)
        };

        var query = dummyData.AsQueryable();

        // Act
        var actualQuery = query.ApplyFilter(filter);
        var expectedQuery = query.Where(x => x.Views.Value <= max);

        // Assert
        Assert.Equal(expectedQuery.Count(), actualQuery.Count());
    }

    [Theory, AutoMoqData(count: 64)]
    public void BuildExpression_MinMaxWithNullable_ShouldMatchCount(List<Book> dummyData, int min)
    {
        // Arrange
        var max = min + random.Next(0, 20);
        var filter = new BookFilter_Range_Views
        {
            Views = new Range<int>(min, max)
        };

        var query = dummyData.AsQueryable();

        // Act
        var actualQuery = query.ApplyFilter(filter);
        var expectedQuery = query.Where(x => min <= x.Views.Value && x.Views.Value <= max);

        // Assert
        Assert.Equal(expectedQuery.Count(), actualQuery.Count());
    }

    [Theory, AutoMoqData(count: 64)]
    public void BuildExpression_MinMaxWithCompareTo_ShouldCompareMultipleProperty(List<Book> dummyData, int min)
    {
        // Arrange
        var max = min + random.Next(0, 20);
        var filter = new BookFilter_Range_WithAttribute
        {
            Value = new Range<int>(min, max)
        };

        var query = dummyData.AsQueryable();

        // Act
        var actualQuery = query.ApplyFilter(filter);
        var expectedQuery = query.Where(x => min <= x.TotalPage && x.TotalPage <= max || min <= x.ReadCount && x.ReadCount <= max);

        // Assert
        Assert.Equal(expectedQuery.Count(), actualQuery.Count());
    }
}
