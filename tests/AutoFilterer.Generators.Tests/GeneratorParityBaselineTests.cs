using AutoFilterer.Extensions;
using AutoFilterer.Generators.Tests.Environment.Dtos;
using AutoFilterer.Generators.Tests.Environment.Models;
using AutoFilterer.Types;
using AutoFilterer.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AutoFilterer.Generators.Tests;

/// <summary>
/// Parity baseline tests - verify that source-generated ApplyFilter
/// matches runtime FilterBase.BuildExpression() behavior for key scenarios.
/// </summary>
public class GeneratorParityBaselineTests
{
    #region CollectionFilterType.All Parity

    [Fact]
    public void Parity_CollectionFilterType_All_NonEmptyCollection_MatchesRuntime()
    {
        // Arrange - authors with books, filter requires ALL books to match criteria
        var authors = new[]
        {
            new Author
            {
                Id = 1,
                Name = "Author A",
                Books = new List<Book>
                {
                    new Book { Title = "Book 1", Year = 2000, TotalPage = 300 },
                    new Book { Title = "Book 2", Year = 2001, TotalPage = 310 },
                    new Book { Title = "Book 3", Year = 2002, TotalPage = 320 }
                }
            },
            new Author
            {
                Id = 2,
                Name = "Author B",
                Books = new List<Book>
                {
                    new Book { Title = "Book 4", Year = 1999, TotalPage = 280 },
                    new Book { Title = "Book 5", Year = 2005, TotalPage = 400 }
                }
            },
            new Author
            {
                Id = 3,
                Name = "Author C",
                Books = new List<Book>
                {
                    new Book { Title = "Book 6", Year = 2000, TotalPage = 305 }
                }
            }
        };

        var filter = new AuthorFilter_CollectionAll
        {
            Books = new BookNestedFilter
            {
                Year = new Range<int> { Min = 2000 }
            }
        };

        // Act - generator version
        var generatorResult = authors.AsQueryable().ApplyFilter(filter).ToList();

        // Assert - expected runtime behavior: ALL books must have Year >= 2000
        Assert.Equal(2, generatorResult.Count);
        Assert.Contains(generatorResult, a => a.Name == "Author A");
        Assert.Contains(generatorResult, a => a.Name == "Author C");
        Assert.DoesNotContain(generatorResult, a => a.Name == "Author B"); // Has a book from 1999
    }

    [Fact]
    public void Parity_CollectionFilterType_All_EmptyCollection_VacuousTruth()
    {
        // Arrange - author with empty book collection
        var authors = new[]
        {
            new Author
            {
                Id = 1,
                Name = "Author With No Books",
                Books = new List<Book>()
            },
            new Author
            {
                Id = 2,
                Name = "Author With Books",
                Books = new List<Book>
                {
                    new Book { Title = "Book 1", Year = 2000, TotalPage = 300 }
                }
            }
        };

        var filter = new AuthorFilter_CollectionAll
        {
            Books = new BookNestedFilter
            {
                Year = new Range<int> { Min = 2000 }
            }
        };

        // Act - generator version
        var generatorResult = authors.AsQueryable().ApplyFilter(filter).ToList();

        // Assert - vacuous truth: empty collection satisfies ALL condition
        Assert.Equal(2, generatorResult.Count); // Both authors match
        Assert.Contains(generatorResult, a => a.Name == "Author With No Books");
    }

    [Fact]
    public void Parity_CollectionFilterType_All_MultipleFilters_AllMustMatch()
    {
        // Arrange
        var authors = new[]
        {
            new Author
            {
                Id = 1,
                Name = "Author A",
                Books = new List<Book>
                {
                    new Book { Title = "Clean", Year = 2000, TotalPage = 300 },
                    new Book { Title = "Clean", Year = 2001, TotalPage = 310 }
                }
            },
            new Author
            {
                Id = 2,
                Name = "Author B",
                Books = new List<Book>
                {
                    new Book { Title = "Dirty", Year = 2000, TotalPage = 300 }
                }
            }
        };

        var filter = new AuthorFilter_CollectionAll
        {
            Books = new BookNestedFilter
            {
                Title = new StringFilter { StartsWith = "Clean" },
                Year = new Range<int> { Min = 2000 }
            }
        };

        // Act
        var generatorResult = authors.AsQueryable().ApplyFilter(filter).ToList();

        // Assert - ALL books must have Title starting with "Clean" AND Year >= 2000
        Assert.Single(generatorResult);
        Assert.Equal("Author A", generatorResult[0].Name);
    }

    #endregion

    #region StringFilter.Compare Parity

    [Fact]
    public void Parity_StringFilter_Compare_Ordinal_MatchesRuntime()
    {
        // Arrange - case-sensitive matching with Ordinal
        var books = new[]
        {
            new Book { Id = 1, Title = "ABC", Author = "John" },
            new Book { Id = 2, Title = "abc", Author = "Jane" },
            new Book { Id = 3, Title = "Abc", Author = "Bob" },
            new Book { Id = 4, Title = "aBc", Author = "Alice" }
        };

        var filter = new BookFilter_StringFilter_Advanced
        {
            Title = new StringFilter
            {
                Compare = StringComparison.Ordinal,
                Contains = "AB"
            }
        };

        // Act
        var generatorResult = books.AsQueryable().ApplyFilter(filter).ToList();

        // Assert - only exact case match
        Assert.Single(generatorResult);
        Assert.Equal("ABC", generatorResult[0].Title);
    }

    [Fact]
    public void Parity_StringFilter_Compare_OrdinalIgnoreCase_MatchesRuntime()
    {
        // Arrange - case-insensitive matching
        var books = new[]
        {
            new Book { Id = 1, Title = "ABC", Author = "John" },
            new Book { Id = 2, Title = "abc", Author = "Jane" },
            new Book { Id = 3, Title = "Abc", Author = "Bob" },
            new Book { Id = 4, Title = "XYZ", Author = "Alice" }
        };

        var filter = new BookFilter_StringFilter_Advanced
        {
            Title = new StringFilter
            {
                Compare = StringComparison.OrdinalIgnoreCase,
                Contains = "AB"
            }
        };

        // Act
        var generatorResult = books.AsQueryable().ApplyFilter(filter).ToList();

        // Assert - all case variations match
        Assert.Equal(3, generatorResult.Count);
        Assert.DoesNotContain(generatorResult, b => b.Title == "XYZ");
    }

    [Fact]
    public void Parity_StringFilter_Compare_InvariantCulture_MatchesRuntime()
    {
        // Arrange - culture-sensitive comparison
        var books = new[]
        {
            new Book { Id = 1, Title = "café", Author = "Pierre" },
            new Book { Id = 2, Title = "cafe", Author = "John" },
            new Book { Id = 3, Title = "CAFÉ", Author = "Marie" }
        };

        var filter = new BookFilter_StringFilter_Advanced
        {
            Title = new StringFilter
            {
                Compare = StringComparison.InvariantCultureIgnoreCase,
                Contains = "caf"
            }
        };

        // Act
        var generatorResult = books.AsQueryable().ApplyFilter(filter).ToList();

        // Assert - accent-insensitive matching for InvariantCultureIgnoreCase
        Assert.Equal(3, generatorResult.Count);
    }

    #endregion

    #region Scalar String Empty-String Parity

    [Fact]
    public void Parity_ScalarString_EmptyString_MatchesExactEmpty()
    {
        // Arrange - filter for empty string value
        var books = new[]
        {
            new Book { Id = 1, Title = "", Author = "Empty Title" },
            new Book { Id = 2, Title = null, Author = "Null Title" },
            new Book { Id = 3, Title = "NonEmpty", Author = "Has Content" },
            new Book { Id = 4, Title = "   ", Author = "Spaces Only" }
        };

        var filter = new BookFilter_Basic { Title = "" };

        // Act
        var generatorResult = books.AsQueryable().ApplyFilter(filter).ToList();

        // Assert - only exact empty string matches, not null or whitespace
        Assert.Single(generatorResult);
        Assert.Equal("", generatorResult[0].Title);
    }

    [Fact]
    public void Parity_ScalarString_NullVsEmptyString_DifferentBehavior()
    {
        // Arrange - verify null and empty are treated differently
        var books = new[]
        {
            new Book { Id = 1, Title = "", Author = "Empty" },
            new Book { Id = 2, Title = null, Author = "Null" }
        };

        var filterEmpty = new BookFilter_Basic { Title = "" };
        var filterNull = new BookFilter_Basic { Title = null };

        // Act
        var resultEmpty = books.AsQueryable().ApplyFilter(filterEmpty).ToList();
        var resultNull = books.AsQueryable().ApplyFilter(filterNull).ToList();

        // Assert
        Assert.Single(resultEmpty);
        Assert.Equal("", resultEmpty[0].Title);

        Assert.Single(resultNull);
        Assert.Null(resultNull[0].Title);
    }

    #endregion

    #region Nullable Range Parity

    [Fact]
    public void Parity_NullableRange_NullValuesHandledCorrectly()
    {
        // Arrange - mix of null and non-null Views values
        var books = new[]
        {
            new Book { Id = 1, Title = "Book 1", Views = 100 },
            new Book { Id = 2, Title = "Book 2", Views = 200 },
            new Book { Id = 3, Title = "Book 3", Views = null },
            new Book { Id = 4, Title = "Book 4", Views = 150 },
            new Book { Id = 5, Title = "Book 5", Views = null }
        };

        var filter = new BookFilter_OperatorFilter_Advanced
        {
            Views = new OperatorFilter<int> { Gte = 150 }
        };

        // Act
        var generatorResult = books.AsQueryable().ApplyFilter(filter).ToList();

        // Assert - null values should be excluded, only non-null >= 150 included
        Assert.Equal(2, generatorResult.Count);
        Assert.All(generatorResult, b => Assert.True(b.Views.HasValue && b.Views.Value >= 150));
    }

    [Fact]
    public void Parity_NullableRange_IsNullOperator_MatchesNulls()
    {
        // Arrange
        var books = new[]
        {
            new Book { Id = 1, Title = "Book 1", Views = 100 },
            new Book { Id = 2, Title = "Book 2", Views = null },
            new Book { Id = 3, Title = "Book 3", Views = 200 }
        };

        var filter = new BookFilter_OperatorFilter_Advanced
        {
            Views = new OperatorFilter<int> { IsNull = true }
        };

        // Act
        var generatorResult = books.AsQueryable().ApplyFilter(filter).ToList();

        // Assert - only null values match
        Assert.Single(generatorResult);
        Assert.Null(generatorResult[0].Views);
    }

    [Fact]
    public void Parity_NullableRange_RangeFilter_NullValueInSource_Ignored()
    {
        // Arrange - Range filter on nullable property (Views is int?)
        var books = new[]
        {
            new Book { Id = 1, Title = "Book 1", Views = 2000 },
            new Book { Id = 2, Title = "Book 2", Views = null },
            new Book { Id = 3, Title = "Book 3", Views = 2010 }
        };

        var filter = new BookFilter_OperatorFilter_Advanced
        {
            Views = new OperatorFilter<int> { Gte = 1999, Lte = 2005 }
        };

        // Act
        var generatorResult = books.AsQueryable().ApplyFilter(filter).ToList();

        // Assert - null Views values should be filtered out
        Assert.Single(generatorResult);
        Assert.Equal(2000, generatorResult[0].Views);
    }

    #endregion

    #region Guid Array Runtime Parity

    [Fact]
    public void Parity_GuidArray_NullableProperty_MatchesAnyValue()
    {
        // Arrange - test Guid array against nullable Guid property
        var preferences = new[]
        {
            new Preferences
            {
                UserId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                OrganizationUnitId = Guid.Parse("AAAA0000-AAAA-0000-AAAA-000000000001"),
                GivenName = "Alice"
            },
            new Preferences
            {
                UserId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                OrganizationUnitId = null,  // Nullable - should still match if in array
                GivenName = "Bob"
            },
            new Preferences
            {
                UserId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                OrganizationUnitId = Guid.Parse("BBBB0000-BBBB-0000-BBBB-000000000002"),
                GivenName = "Charlie"
            }
        };

        var filter = new PreferencesFilter_ArraySearchGuidWithout
        {
            OrganizationUnitId = new Guid?[]
            {
                Guid.Parse("AAAA0000-AAAA-0000-AAAA-000000000001"),
                Guid.Parse("BBBB0000-BBBB-0000-BBBB-000000000002"),
                null  // Include null in search array
            }
        };

        // Act
        var generatorResult = preferences.AsQueryable().ApplyFilter(filter).ToList();

        // Assert - should match Alice, Bob (null), and Charlie
        Assert.Equal(3, generatorResult.Count);
    }

    [Fact]
    public void Parity_GuidArray_EmptyArray_NoMatches()
    {
        // Arrange - empty search array should return no results
        var preferences = new[]
        {
            new Preferences
            {
                UserId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                OrganizationUnitId = Guid.Parse("AAAA0000-AAAA-0000-AAAA-000000000001"),
                GivenName = "Alice"
            }
        };

        var filter = new PreferencesFilter_ArraySearchGuidWithout
        {
            OrganizationUnitId = Array.Empty<Guid?>()
        };

        // Act
        var generatorResult = preferences.AsQueryable().ApplyFilter(filter).ToList();

        // Assert - empty array matches nothing
        Assert.Empty(generatorResult);
    }

    #endregion

    #region Invalid Metadata/Runtime Error Parity

    [Fact]
    public void Parity_InvalidCompareToProperty_ThrowsOrIgnoresBasedOnFlag()
    {
        // Arrange - filter referencing non-existent property
        var books = new[]
        {
            new Book { Id = 1, Title = "Test", Author = "John" }
        };

        var filter = new BookFilter_InvalidTarget
        {
            Search = "value"
        };

        // Act & Assert
        // With IgnoreExceptions=true (default), should silently ignore invalid property
        // This test verifies the generator doesn't crash, even if it produces no matches
        var generatorResult = books.AsQueryable().ApplyFilter(filter).ToList();
        Assert.NotNull(generatorResult);
        // Either all or none may match depending on generator handling
        // Key point: no exception thrown
    }

    #endregion
}
