using AutoFilterer.Dynamics.Tests.Environment.Models;
using AutoFilterer.Extensions;
using AutoFilterer.Tests.Core;
#if LEGACY_NAMESPACE
using AutoFilterer.Enums;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AutoFilterer.Dynamics.Tests;

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
        DynamicFilter filter = new DynamicFilter { { "TotalPage", "5" }, { "IsPublished", "True" } };
        filter.CombineWith = CombineType.And;

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
        DynamicFilter filter = new DynamicFilter { { "TotalPage", "5" }, { "IsPublished", "True" } };
        filter.CombineWith = CombineType.Or;

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

    [Theory, AutoMoqData(count: 32)]
    public void ApplyFilterTo_ShouldFilterCorrect_WithCustomComparisonKeywordEq(List<Book> list)
    {
        // Arrange
        DynamicFilter filter = new DynamicFilter { { "TotalPage.eq", "5" } };

        // Act
        var actualQuery = list.AsQueryable().ApplyFilter(filter);
        var exprectedQuery = list.AsQueryable().Where(x => x.TotalPage == 5);

        var actualResult = actualQuery.ToList();
        var expectedResult = exprectedQuery.ToList();

        // Assert
        Assert.Equal(exprectedQuery.Expression.ToString(), actualQuery.Expression.ToString());
        Assert.Equal(expectedResult.Count, actualResult.Count);

        foreach (var item in expectedResult)
            Assert.Contains(item, actualResult);
    }

    [Theory, AutoMoqData(count: 32)]
    public void ApplyFilterTo_ShouldFilterCorrect_WithCustomComparisonKeywordGte(List<Book> list)
    {
        // Arrange
        DynamicFilter filter = new DynamicFilter { { "TotalPage.gte", "5" } };

        // Act
        var actualQuery = list.AsQueryable().ApplyFilter(filter);
        var exprectedQuery = list.AsQueryable().Where(x => x.TotalPage >= 5);

        var actualResult = actualQuery.ToList();
        var expectedResult = exprectedQuery.ToList();

        // Assert
        Assert.Equal(exprectedQuery.Expression.ToString(), actualQuery.Expression.ToString());
        Assert.Equal(expectedResult.Count, actualResult.Count);

        foreach (var item in expectedResult)
            Assert.Contains(item, actualResult);
    }

    [Theory, AutoMoqData(count: 32)]
    public void ApplyFilterTo_ShouldFilterCorrect_WithCustomComparisonKeywordGt(List<Book> list)
    {
        // Arrange
        DynamicFilter filter = new DynamicFilter { { "TotalPage.gt", "5" } };

        // Act
        var actualQuery = list.AsQueryable().ApplyFilter(filter);
        var exprectedQuery = list.AsQueryable().Where(x => x.TotalPage > 5);

        var actualResult = actualQuery.ToList();
        var expectedResult = exprectedQuery.ToList();

        // Assert
        Assert.Equal(exprectedQuery.Expression.ToString(), actualQuery.Expression.ToString());
        Assert.Equal(expectedResult.Count, actualResult.Count);

        foreach (var item in expectedResult)
            Assert.Contains(item, actualResult);
    }

    [Theory, AutoMoqData(count: 32)]
    public void ApplyFilterTo_ShouldFilterCorrect_WithCustomComparisonKeywordLt(List<Book> list)
    {
        // Arrange
        DynamicFilter filter = new DynamicFilter { { "TotalPage.lt", "5" } };

        // Act
        var actualQuery = list.AsQueryable().ApplyFilter(filter);
        var exprectedQuery = list.AsQueryable().Where(x => x.TotalPage < 5);

        var actualResult = actualQuery.ToList();
        var expectedResult = exprectedQuery.ToList();

        // Assert
        Assert.Equal(exprectedQuery.Expression.ToString(), actualQuery.Expression.ToString());
        Assert.Equal(expectedResult.Count, actualResult.Count);

        foreach (var item in expectedResult)
            Assert.Contains(item, actualResult);
    }

    [Theory, AutoMoqData(count: 32)]
    public void ApplyFilterTo_ShouldFilterCorrect_WithCustomComparisonKeywordLte(List<Book> list)
    {
        // Arrange
        DynamicFilter filter = new DynamicFilter { { "TotalPage.lte", "5" } };

        // Act
        var actualQuery = list.AsQueryable().ApplyFilter(filter);
        var exprectedQuery = list.AsQueryable().Where(x => x.TotalPage <= 5);

        var actualResult = actualQuery.ToList();
        var expectedResult = exprectedQuery.ToList();

        // Assert
        Assert.Equal(exprectedQuery.Expression.ToString(), actualQuery.Expression.ToString());
        Assert.Equal(expectedResult.Count, actualResult.Count);

        foreach (var item in expectedResult)
            Assert.Contains(item, actualResult);
    }

    [Theory, AutoMoqData(count: 32)]
    public void ApplyFilterTo_ShouldFilterCorrect_WithCustomComparisonKeywordNot(List<Book> list)
    {
        // Arrange
        DynamicFilter filter = new DynamicFilter { { "TotalPage.not", "5" } };

        // Act
        var actualQuery = list.AsQueryable().ApplyFilter(filter);
        var exprectedQuery = list.AsQueryable().Where(x => x.TotalPage != 5);

        var actualResult = actualQuery.ToList();
        var expectedResult = exprectedQuery.ToList();

        // Assert
        Assert.Equal(exprectedQuery.Expression.ToString(), actualQuery.Expression.ToString());
        Assert.Equal(expectedResult.Count, actualResult.Count);

        foreach (var item in expectedResult)
            Assert.Contains(item, actualResult);
    }

    [Theory, AutoMoqData(count: 32)]
    public void ApplyFilterTo_ShouldFilterCorrect_WithCustomComparisonKeywordContains(List<Book> list)
    {
        // Arrange
        DynamicFilter filter = new DynamicFilter { { "Title.contains", "a" } };

        // Act
        var actualQuery = list.AsQueryable().ApplyFilter(filter);
        var exprectedQuery = list.AsQueryable().Where(x => x.Title.Contains("a"));

        var actualResult = actualQuery.ToList();
        var expectedResult = exprectedQuery.ToList();

        // Assert
        Assert.Equal(exprectedQuery.Expression.ToString(), actualQuery.Expression.ToString());
        Assert.Equal(expectedResult.Count, actualResult.Count);

        foreach (var item in expectedResult)
            Assert.Contains(item, actualResult);
    }

    [Theory, AutoMoqData(count: 32)]
    public void ApplyFilterTo_ShouldFilterCorrect_WithCustomComparisonKeywordStartsWith(List<Book> list)
    {
        // Arrange
        DynamicFilter filter = new DynamicFilter { { "Title.startswith", "a" } };

        // Act
        var actualQuery = list.AsQueryable().ApplyFilter(filter);
        var exprectedQuery = list.AsQueryable().Where(x => x.Title.StartsWith("a"));

        var actualResult = actualQuery.ToList();
        var expectedResult = exprectedQuery.ToList();

        // Assert
        Assert.Equal(exprectedQuery.Expression.ToString(), actualQuery.Expression.ToString());
        Assert.Equal(expectedResult.Count, actualResult.Count);

        foreach (var item in expectedResult)
            Assert.Contains(item, actualResult);
    }

    [Theory, AutoMoqData(count: 32)]
    public void ApplyFilterTo_ShouldFilterCorrect_WithCustomComparisonKeywordEndsWith(List<Book> list)
    {
        // Arrange
        DynamicFilter filter = new DynamicFilter { { "Title.endswith", "a" } };

        // Act
        var actualQuery = list.AsQueryable().ApplyFilter(filter);
        var exprectedQuery = list.AsQueryable().Where(x => x.Title.EndsWith("a"));

        var actualResult = actualQuery.ToList();
        var expectedResult = exprectedQuery.ToList();

        // Assert
        Assert.Equal(exprectedQuery.Expression.ToString(), actualQuery.Expression.ToString());
        Assert.Equal(expectedResult.Count, actualResult.Count);

        foreach (var item in expectedResult)
            Assert.Contains(item, actualResult);
    }
}
