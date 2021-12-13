#if LEGACY_NAMESPACE
using AutoFilterer.Enums;
#endif
using AutoFilterer.Tests.Environment.Dtos;
using AutoFilterer.Tests.Environment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoFilterer.Extensions;
using AutoFilterer.Tests.Core;
using Xunit;
using AutoFilterer.Types;
using System.ComponentModel.DataAnnotations;
using AutoFilterer.Attributes;

namespace AutoFilterer.Tests.Attributes;

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

    [Theory, AutoMoqData(count: 3)]
    public void ShouldThrowException_WhenWrongFilterableTypeSet(List<Book> books)
    {
        AutoFiltererConsts.IgnoreExceptions = false;

        var argumentException = Assert.Throws<ArgumentException>(() =>
        {
            books.AsQueryable().ApplyFilter(new WrongTypeSetFilter() { Filter = "A" });
        });
    }

    public class WrongTypeSetFilter : FilterBase
    {
        [CompareTo(typeof(Exception), "Title")]
        public string Filter { get; set; }
    }

    [Theory, AutoMoqData(count: 64)]
    public void ShouldFilterWithTypeInAttribute(List<Book> dummyData)
    {
        // Arrange
        var filter = new TypeCompareToFilter
        {
            Search = "titlea"
        };

        var query = dummyData.AsQueryable();

        var expectedQuery = query.Where(x => x.Title.ToLower().Contains(filter.Search.ToLower()));
        var expected = expectedQuery.ToList();

        // Act
        var actualQuery = query.ApplyFilter(filter);
        var actual = actualQuery.ToList();

        // Assert
        Assert.Equal(expected.Count, actual.Count);
    }

    public class TypeCompareToFilter : FilterBase
    {
        [CompareTo(typeof(ToLowerContainsComparisonAttribute), nameof(Book.Title))]
        public string Search { get; set; }
    }

    [Theory, AutoMoqData(count: 64)]
    public void ShouldFilterWithTypeInAttributeWithMultipleAttribute(List<Book> dummyData)
    {
        // Arrange
        var filter = new MultipleTypeCompareToFilter
        {
            Search = "af"
        };

        var query = dummyData.AsQueryable();

        var expectedQuery = query.Where(x =>
                x.Title.ToLower().Contains(filter.Search.ToLower())
                || x.Author.StartsWith(filter.Search, StringComparison.InvariantCultureIgnoreCase));

        var expected = expectedQuery.ToList();

        // Act
        var actualQuery = query.ApplyFilter(filter);
        var actual = actualQuery.ToList();

        // Assert
        Assert.Equal(expected.Count, actual.Count);
    }

    public class MultipleTypeCompareToFilter : FilterBase
    {
        [CompareTo(typeof(ToLowerContainsComparisonAttribute), nameof(Book.Title))]
        [CompareTo(typeof(StartsWithAttribute), nameof(Book.Author))]
        public string Search { get; set; }

        public class StartsWithAttribute : StringFilterOptionsAttribute
        {
            public StartsWithAttribute() : base(StringFilterOption.StartsWith, StringComparison.InvariantCultureIgnoreCase)
            {
            }
        }
    }

    [Theory, AutoMoqData(count: 64)]
    public void ShouldFilterWithTypeInAttributeWithMultipleAttributeWithAndCombination(List<Book> dummyData)
    {
        // Arrange
        var filter = new MultipleTypeCompareToAndComparisonFilter
        {
            Search = "9"
        };

        var query = dummyData.AsQueryable();

        var expectedQuery = query.Where(x =>
                x.Title.ToLower().Contains(filter.Search.ToLower())
                && x.Author.EndsWith(filter.Search, StringComparison.InvariantCultureIgnoreCase));

        var expected = expectedQuery.ToList();

        // Act
        var actualQuery = query.ApplyFilter(filter);
        var actual = actualQuery.ToList();

        // Assert
        Assert.Equal(expected.Count, actual.Count);
    }

    public class MultipleTypeCompareToAndComparisonFilter : FilterBase
    {
        [CompareTo(typeof(ToLowerContainsComparisonAttribute), nameof(Book.Title))]
        [CompareTo(typeof(EndsWithAttribute), nameof(Book.Author), CombineWith = CombineType.And)]
        public string Search { get; set; }

        public class EndsWithAttribute : StringFilterOptionsAttribute
        {
            public EndsWithAttribute() : base(StringFilterOption.EndsWith, StringComparison.InvariantCultureIgnoreCase)
            {
            }
        }
    }
}
