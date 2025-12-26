using AutoFilterer.Generators.Tests.Environment.Dtos;
using AutoFilterer.Generators.Tests.Environment.Models;
using System.Linq;
using Xunit;

namespace AutoFilterer.Generators.Tests;

/// <summary>
/// These tests validate that the source generator creates compilable code
/// and that the ApplyFilter extension method works correctly.
/// </summary>
public class GeneratorSmokeTests
{
    [Fact]
    public void Generator_CreatesApplyFilterExtension_ForBasicFilter()
    {
        // Arrange
        var data = TestDataHelper.GetSampleBooks();
        var filter = new BookFilter_Basic { Title = "Clean Code" };

        // Act - This will only compile if the generator created the ApplyFilter extension
        var result = data.AsQueryable().ApplyFilter(filter).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("Clean Code", result[0].Title);
    }

    [Fact]
    public void Generator_CreatesApplyFilterExtension_ForStringFilter()
    {
        // Arrange
        var data = TestDataHelper.GetSampleBooks();
        var filter = new BookFilter_StringFilter
        {
            Title = new AutoFilterer.Types.StringFilter { Eq = "Clean Code" }
        };

        // Act
        var result = data.AsQueryable().ApplyFilter(filter).ToList();

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public void Generator_CreatesApplyFilterExtension_ForOperatorFilter()
    {
        // Arrange
        var data = TestDataHelper.GetSampleBooks();
        var filter = new BookFilter_OperatorFilter
        {
            TotalPage = new AutoFilterer.Types.OperatorFilter<int> { Gt = 400 }
        };

        // Act
        var result = data.AsQueryable().ApplyFilter(filter).ToList();

        // Assert
        Assert.True(result.Count > 0);
        Assert.All(result, book => Assert.True(book.TotalPage > 400));
    }

    [Fact]
    public void Generator_CreatesApplyFilterExtension_ForRangeFilter()
    {
        // Arrange
        var data = TestDataHelper.GetSampleBooks();
        var filter = new BookFilter_Range
        {
            Year = new AutoFilterer.Types.Range<int> { Min = 2000, Max = 2010 }
        };

        // Act
        var result = data.AsQueryable().ApplyFilter(filter).ToList();

        // Assert
        Assert.True(result.Count > 0);
        Assert.All(result, book => Assert.True(book.Year >= 2000 && book.Year <= 2010));
    }

    [Fact]
    public void Generator_CreatesApplyFilterExtension_ForOrderableFilter()
    {
        // Arrange
        var data = TestDataHelper.GetSampleBooks();
        var filter = new BookFilter_Orderable
        {
            SortBy = AutoFilterer.Enums.Sorting.Descending,
            Sort = nameof(Environment.Models.Book.Year)
        };

        // Act
        var result = data.AsQueryable().ApplyFilter(filter).ToList();

        // Assert
        Assert.Equal(data.Count, result.Count);
        // Check if sorted descending by year
        for (int i = 0; i < result.Count - 1; i++)
        {
            Assert.True(result[i].Year >= result[i + 1].Year);
        }
    }

    [Fact]
    public void Generator_CreatesApplyFilterExtension_ForPaginationFilter()
    {
        // Arrange
        var data = TestDataHelper.GetSampleBooks();
        var filter = new BookFilter_Pagination
        {
            Page = 2,
            PerPage = 3
        };

        // Act
        var result = data.AsQueryable().ApplyFilter(filter).ToList();

        // Assert
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void Generator_CreatesApplyFilterExtension_ForCollectionFilter()
    {
        // Arrange
        var data = TestDataHelper.GetSampleAuthors();
        var filter = new AuthorFilter_CollectionAny
        {
            Books = new BookNestedFilter
            {
                Title = new AutoFilterer.Types.StringFilter { Eq = "Clean Code" }
            }
        };

        // Act
        var result = data.AsQueryable().ApplyFilter(filter).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("Robert C. Martin", result[0].Name);
    }

    [Fact]
    public void Generator_CreatesApplyFilterExtension_ForNestedCollectionFilter()
    {
        // Arrange
        var data = TestDataHelper.GetSamplePublishers();
        var filter = new PublisherFilter_NestedCollection
        {
            Authors = new AuthorNestedFilter
            {
                Books = new BookNestedFilter
                {
                    Year = new AutoFilterer.Types.Range<int> { Min = 2008, Max = 2008 }
                }
            }
        };

        // Act
        var result = data.AsQueryable().ApplyFilter(filter).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("Pearson", result[0].Name);
    }

    [Fact]
    public void Generator_CreatesApplyFilterExtension_ForCompleteFilter()
    {
        // Arrange
        var data = TestDataHelper.GetSampleAuthors();
        var filter = new AuthorFilter_Complete
        {
            Country = "USA",
            Books = new BookNestedFilter
            {
                Year = new AutoFilterer.Types.Range<int> { Min = 2008, Max = 2008 }
            },
            Page = 1,
            PerPage = 10
        };

        // Act
        var result = data.AsQueryable().ApplyFilter(filter).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("Robert C. Martin", result[0].Name);
    }
}
