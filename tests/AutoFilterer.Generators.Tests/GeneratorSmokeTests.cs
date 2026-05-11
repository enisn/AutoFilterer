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

    [Fact]
    public void Generator_AppliesStringFilter_AllStringMembers()
    {
        // Arrange
        var books = new[]
        {
            new Book { Id = 1, Title = null, TotalPage = 100, Views = null },
            new Book { Id = 2, Title = string.Empty, TotalPage = 200, Views = 10 },
            new Book { Id = 3, Title = "Alpha", TotalPage = 300, Views = 20 },
            new Book { Id = 4, Title = "AlphaBeta", TotalPage = 400, Views = null },
            new Book { Id = 5, Title = "Beta", TotalPage = 500, Views = 30 },
            new Book { Id = 6, Title = "Gamma", TotalPage = 600, Views = 40 },
        };

        var eq = books.AsQueryable().ApplyFilter(new BookFilter_StringFilter_Advanced { Title = new AutoFilterer.Types.StringFilter { Eq = "Alpha" } }).ToList();
        var not = books.AsQueryable().ApplyFilter(new BookFilter_StringFilter_Advanced { Title = new AutoFilterer.Types.StringFilter { Not = "Alpha" } }).ToList();
        var equals = books.AsQueryable().ApplyFilter(new BookFilter_StringFilter_Advanced { Title = new AutoFilterer.Types.StringFilter { Equals = "AlphaBeta" } }).ToList();
        var contains = books.AsQueryable().ApplyFilter(new BookFilter_StringFilter_Advanced { Title = new AutoFilterer.Types.StringFilter { Contains = "Alpha" } }).ToList();
        var notContains = books.AsQueryable().ApplyFilter(new BookFilter_StringFilter_Advanced { Title = new AutoFilterer.Types.StringFilter { NotContains = "Alpha" } }).ToList();
        var startsWith = books.AsQueryable().ApplyFilter(new BookFilter_StringFilter_Advanced { Title = new AutoFilterer.Types.StringFilter { StartsWith = "Al" } }).ToList();
        var notStartsWith = books.AsQueryable().ApplyFilter(new BookFilter_StringFilter_Advanced { Title = new AutoFilterer.Types.StringFilter { NotStartsWith = "Al" } }).ToList();
        var endsWith = books.AsQueryable().ApplyFilter(new BookFilter_StringFilter_Advanced { Title = new AutoFilterer.Types.StringFilter { EndsWith = "ta" } }).ToList();
        var notEndsWith = books.AsQueryable().ApplyFilter(new BookFilter_StringFilter_Advanced { Title = new AutoFilterer.Types.StringFilter { NotEndsWith = "ta" } }).ToList();
        var isNull = books.AsQueryable().ApplyFilter(new BookFilter_StringFilter_Advanced { Title = new AutoFilterer.Types.StringFilter { IsNull = true } }).ToList();
        var isNotNull = books.AsQueryable().ApplyFilter(new BookFilter_StringFilter_Advanced { Title = new AutoFilterer.Types.StringFilter { IsNotNull = true } }).ToList();
        var isEmpty = books.AsQueryable().ApplyFilter(new BookFilter_StringFilter_Advanced { Title = new AutoFilterer.Types.StringFilter { IsEmpty = true } }).ToList();
        var isNotEmpty = books.AsQueryable().ApplyFilter(new BookFilter_StringFilter_Advanced { Title = new AutoFilterer.Types.StringFilter { IsNotEmpty = true } }).ToList();
        var andCombined = books.AsQueryable().ApplyFilter(new BookFilter_StringFilter_Advanced
        {
            Title = new AutoFilterer.Types.StringFilter { Contains = "Alpha", EndsWith = "ta", CombineWith = AutoFilterer.Enums.CombineType.And }
        }).ToList();
        var orCombined = books.AsQueryable().ApplyFilter(new BookFilter_StringFilter_Advanced
        {
            Title = new AutoFilterer.Types.StringFilter { Contains = "Alpha", StartsWith = "Ga", CombineWith = AutoFilterer.Enums.CombineType.Or }
        }).ToList();

        // Nested path coverage (BookNestedFilter.Title)
        var authors = new[]
        {
            new Author { Id = 1, Name = "A", Books = books.Where(x => x.Id > 1 && x.Id <= 3).ToList() },
            new Author { Id = 2, Name = "B", Books = books.Where(x => x.Id >= 4).ToList() }
        };
        var nestedAnd = authors.AsQueryable().ApplyFilter(new AuthorFilter_CollectionAny
        {
            Books = new BookNestedFilter
            {
                Title = new AutoFilterer.Types.StringFilter { Contains = "Alpha", EndsWith = "ta", CombineWith = AutoFilterer.Enums.CombineType.And }
            }
        }).ToList();

        // Assert
        Assert.Single(eq);
        Assert.Equal(5, not.Count);
        Assert.Single(equals);
        Assert.Equal(2, contains.Count);
        Assert.Equal(4, notContains.Count);
        Assert.Equal(2, startsWith.Count);
        Assert.Equal(4, notStartsWith.Count);
        Assert.Equal(2, endsWith.Count);
        Assert.Equal(4, notEndsWith.Count);
        Assert.Single(isNull);
        Assert.Equal(5, isNotNull.Count);
        Assert.Single(isEmpty);
        Assert.Equal(5, isNotEmpty.Count);
        Assert.Single(andCombined);
        Assert.Equal("AlphaBeta", andCombined[0].Title);
        Assert.Equal(3, orCombined.Count);
        Assert.Single(nestedAnd);
        Assert.Equal("B", nestedAnd[0].Name);
    }

    [Fact]
    public void Generator_AppliesOperatorFilter_AllOperatorMembers()
    {
        // Arrange
        var books = new[]
        {
            new Book { Id = 1, TotalPage = 100, Views = null },
            new Book { Id = 2, TotalPage = 200, Views = 10 },
            new Book { Id = 3, TotalPage = 300, Views = 20 },
            new Book { Id = 4, TotalPage = 400, Views = null },
            new Book { Id = 5, TotalPage = 500, Views = 30 },
        };

        var eq = books.AsQueryable().ApplyFilter(new BookFilter_OperatorFilter_Advanced { TotalPage = new AutoFilterer.Types.OperatorFilter<int> { Eq = 300 } }).ToList();
        var not = books.AsQueryable().ApplyFilter(new BookFilter_OperatorFilter_Advanced { TotalPage = new AutoFilterer.Types.OperatorFilter<int> { Not = 300 } }).ToList();
        var gt = books.AsQueryable().ApplyFilter(new BookFilter_OperatorFilter_Advanced { TotalPage = new AutoFilterer.Types.OperatorFilter<int> { Gt = 300 } }).ToList();
        var lt = books.AsQueryable().ApplyFilter(new BookFilter_OperatorFilter_Advanced { TotalPage = new AutoFilterer.Types.OperatorFilter<int> { Lt = 300 } }).ToList();
        var gte = books.AsQueryable().ApplyFilter(new BookFilter_OperatorFilter_Advanced { TotalPage = new AutoFilterer.Types.OperatorFilter<int> { Gte = 300 } }).ToList();
        var lte = books.AsQueryable().ApplyFilter(new BookFilter_OperatorFilter_Advanced { TotalPage = new AutoFilterer.Types.OperatorFilter<int> { Lte = 300 } }).ToList();
        var isNull = books.AsQueryable().ApplyFilter(new BookFilter_OperatorFilter_Advanced { Views = new AutoFilterer.Types.OperatorFilter<int> { IsNull = true } }).ToList();
        var isNotNull = books.AsQueryable().ApplyFilter(new BookFilter_OperatorFilter_Advanced { Views = new AutoFilterer.Types.OperatorFilter<int> { IsNotNull = true } }).ToList();
        var andCombined = books.AsQueryable().ApplyFilter(new BookFilter_OperatorFilter_Advanced
        {
            Views = new AutoFilterer.Types.OperatorFilter<int> { Gte = 15, Lte = 25, CombineWith = AutoFilterer.Enums.CombineType.And }
        }).ToList();
        var orCombined = books.AsQueryable().ApplyFilter(new BookFilter_OperatorFilter_Advanced
        {
            Views = new AutoFilterer.Types.OperatorFilter<int> { Gt = 25, Lt = 15, CombineWith = AutoFilterer.Enums.CombineType.Or }
        }).ToList();
        var nullOnNonNullable = books.AsQueryable().ApplyFilter(new BookFilter_OperatorFilter_Advanced
        {
            TotalPage = new AutoFilterer.Types.OperatorFilter<int> { IsNull = true, IsNotNull = true }
        }).ToList();

        // Nested path coverage (BookNestedFilter.Views/TotalPage)
        var authors = new[]
        {
            new Author { Id = 1, Name = "A", Books = books.Where(x => x.Id <= 2).ToList() },
            new Author { Id = 2, Name = "B", Books = books.Where(x => x.Id == 3 || x.Id == 5).ToList() },
            new Author { Id = 3, Name = "C", Books = books.Where(x => x.Id == 4).ToList() },
        };
        var nestedNullableNull = authors.AsQueryable().ApplyFilter(new AuthorFilter_CollectionAny
        {
            Books = new BookNestedFilter
            {
                Views = new AutoFilterer.Types.OperatorFilter<int> { IsNull = true }
            }
        }).ToList();
        var nestedNonNullableNullIgnored = authors.AsQueryable().ApplyFilter(new AuthorFilter_CollectionAny
        {
            Books = new BookNestedFilter
            {
                TotalPage = new AutoFilterer.Types.OperatorFilter<int> { IsNull = true }
            }
        }).ToList();

        // Assert
        Assert.Single(eq);
        Assert.Equal(4, not.Count);
        Assert.Equal(2, gt.Count);
        Assert.Equal(2, lt.Count);
        Assert.Equal(3, gte.Count);
        Assert.Equal(3, lte.Count);
        Assert.Equal(2, isNull.Count);
        Assert.Equal(3, isNotNull.Count);
        Assert.Single(andCombined);
        Assert.Equal(20, andCombined[0].Views);
        Assert.Equal(2, orCombined.Count);
        Assert.Equal(5, nullOnNonNullable.Count);
        Assert.Equal(2, nestedNullableNull.Count);
        Assert.Equal(3, nestedNonNullableNullIgnored.Count);
    }

    [Fact]
    public void Generator_AppliesStringFilterOptions_OnStringProperty()
    {
        // Arrange
        var data = TestDataHelper.GetSampleBooks();
        var filter = new BookFilter_StringOptionsContains { Query = "Clean" };

        // Act
        var result = data.AsQueryable().ApplyFilter(filter).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, x => Assert.Contains("Clean", x.Title));
    }

    [Fact]
    public void Generator_AppliesArraySearch_AndCompareToFilterableType()
    {
        // Arrange
        var preferences = TestDataHelper.GetSamplePreferences();
        var books = TestDataHelper.GetSampleBooks();

        // Act + Assert array search without attribute
        var arrayWithoutResult = preferences.AsQueryable().ApplyFilter(new PreferencesFilter_ArraySearchWithout
        {
            SecurityLevel = new[] { 1, 3 }
        }).ToList();
        Assert.Equal(3, arrayWithoutResult.Count);

        // Act + Assert array search with attribute
        var arrayWithResult = preferences.AsQueryable().ApplyFilter(new PreferencesFilter_ArraySearchWith
        {
            SecurityLevel = new[] { 2 }
        }).ToList();
        Assert.Single(arrayWithResult);
        Assert.Equal("Bob", arrayWithResult[0].GivenName);

        // Act + Assert compareTo filterable type (ToLowerContainsComparisonAttribute)
        var typeCompareResult = books.AsQueryable().ApplyFilter(new BookFilter_TypeCompareTo
        {
            Search = "clean"
        }).ToList();
        Assert.Equal(2, typeCompareResult.Count);
    }

    [Fact]
    public void Generator_AppliesCompareToOr_ForMultipleStringTargets()
    {
        // Arrange
        var data = TestDataHelper.GetSampleBooks();

        // Act
        var result = data.AsQueryable().ApplyFilter(new BookFilter_MultiplePropertyOr
        {
            Query = "Pragmatic"
        }).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("The Pragmatic Programmer", result[0].Title);
    }

    [Fact]
    public void Generator_AppliesCompareToOr_ForMultipleRangeTargets()
    {
        // Arrange
        var data = TestDataHelper.GetSampleBooks();

        // Act
        var result = data.AsQueryable().ApplyFilter(new BookFilter_Range_MultipleProperty
        {
            PageRange = new AutoFilterer.Types.Range<int> { Min = 550 }
        }).ToList();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Generator_AppliesInheritedIgnoreFilterAttribute()
    {
        // Arrange
        var data = TestDataHelper.GetSampleBooks();

        // Act
        var result = data.AsQueryable().ApplyFilter(new BookFilter_InheritedIgnore
        {
            IgnoredQuery = "Nonexistent"
        }).ToList();

        // Assert
        Assert.Equal(data.Count, result.Count);
    }

    [Fact]
    public void Generator_AppliesCompareToDottedPathMemberAccess()
    {
        // Arrange
        var data = TestDataHelper.GetSampleAuthors().SelectMany(a => a.Books).ToList();

        // Act
        var result = data.AsQueryable().ApplyFilter(new BookFilter_DottedPath
        {
            AuthorName = "andrew"
        }).ToList();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.All(result, b => Assert.Equal("Andrew Hunt", b.AuthorModel.Name));
    }
}
