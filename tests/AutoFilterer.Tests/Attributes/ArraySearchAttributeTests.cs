using AutoFilterer.Extensions;
using AutoFilterer.Tests.Core;
using AutoFilterer.Tests.Environment.Dtos;
using AutoFilterer.Tests.Environment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AutoFilterer.Tests.Attributes;

public class ArraySearchAttributeTests
{
    private static readonly Random random = new Random();
    [Theory, AutoMoqData(count: 64)]
    public void BuildExpression_ShouldGenerateQueryCorrect_WithoutAttribute(List<Preferences> data)
    {
        // Arrange
        var queryable = data.AsQueryable();
        var filter = new PreferencesFilter_ArraySearchWithoutAttribute
        {
            SecurityLevel = Repeat(() => random.Next(200), 45).ToArray()
        };

        var value = filter.SecurityLevel.Select(s => s).ToArray();

        // Act
        var expectedResult = queryable.Where(x => value.Contains(x.SecurityLevel));
        var actualResult = queryable.ApplyFilter(filter);

        // Assert

        Assert.Equal(expectedResult.Count(), actualResult.Count());

        foreach (var expected in expectedResult)
            Assert.True(actualResult.Contains(expected));
    }

    [Theory, AutoMoqData(count: 64)]
    public void BuildExpression_ShouldGenerateQueryCorrect_WithAttribute(List<Preferences> data)
    {
        // Arrange
        var queryable = data.AsQueryable();
        var filter = new PreferencesFilter_ArraySearchWithAttribute
        {
            SecurityLevel = Repeat(() => random.Next(200), 45).ToArray()
        };

        // Act
        var expectedResult = queryable.Where(x => filter.SecurityLevel.Contains(x.SecurityLevel));
        var actualResult = queryable.ApplyFilter(filter);

        // Assert
        Assert.Equal(expectedResult.Count(), actualResult.Count());

        foreach (var expected in expectedResult)
            Assert.True(actualResult.Contains(expected));
    }

    private static IEnumerable<T> Repeat<T>(Func<T> func, int times = 3)
    {
        for (int i = 0; i < times; i++)
        {
            yield return func();
        }
    }
}
