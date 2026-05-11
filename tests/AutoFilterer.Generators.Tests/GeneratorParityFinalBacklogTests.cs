using AutoFilterer.Enums;
using AutoFilterer.Extensions;
using AutoFilterer.Generators.Tests.Environment.Dtos;
using AutoFilterer.Generators.Tests.Environment.Models;
using AutoFilterer.Types;
using Author = AutoFilterer.Generators.Tests.Environment.Models.Author;
using Publisher = AutoFilterer.Generators.Tests.Environment.Models.Publisher;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AutoFilterer.Generators.Tests;

/// <summary>
/// Final strict parity tests covering the remaining backlog gaps:
/// 1) CompareTo multi-target OR semantics
/// 2) int[] ArraySearch parity with and without attribute
/// 3) single typed CompareTo(typeof(...)) parity
/// 4) StringFilter.NotContains + Compare mode
/// 5) stronger non-nullable Range<T> bound parity with meaningful dataset
/// </summary>
public class GeneratorParityFinalBacklogTests
{
    #region CompareTo Multi-Target OR Semantics

    [Fact]
    public void Parity_CompareToMultipleTargets_OR_Semantics_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Clean Code", Author = "Robert Martin" },
            new Book { Id = 2, Title = "Clean Architecture", Author = "Other Author" },
            new Book { Id = 3, Title = "Code Complete", Author = "Steve McConnell" },
            new Book { Id = 4, Title = "Refactoring", Author = "Martin Fowler" },
            new Book { Id = 5, Title = "Book Title", Author = "Martin Smith" }
        };

        var filter = new BookFilter_MultiplePropertyOr { Query = "Martin" };
        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_CompareToMultipleTargets_OR_MatchesWhenAnyTargetMatches()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Martin's Book", Author = "Other" },
            new Book { Id = 2, Title = "Book Title", Author = "Martin Smith" },
            new Book { Id = 3, Title = "Neither", Author = "Matches" },
            new Book { Id = 4, Title = "Martin Book", Author = "Martin" }
        };

        var filter = new BookFilter_MultiplePropertyOr { Query = "Martin" };
        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_CompareToMultipleTargets_OR_CaseSensitivity_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "CLEAN code", Author = "John" },
            new Book { Id = 2, Title = "clean CODE", Author = "Jane" },
            new Book { Id = 3, Title = "Other", Author = "CLEAN" },
            new Book { Id = 4, Title = "None", Author = "Other" }
        };

        var filter = new BookFilter_MultiplePropertyOr { Query = "clean" };
        AssertParity(books, filter);
    }

    #endregion

    #region int[] ArraySearch Parity (With and Without Attribute)

    [Fact]
    public void Parity_IntArray_WithoutAttribute_MatchesRuntime()
    {
        var preferences = new[]
        {
            new Preferences { UserId = Guid.Parse("11111111-1111-1111-1111-111111111111"), SecurityLevel = 1, GivenName = "Alice" },
            new Preferences { UserId = Guid.Parse("22222222-2222-2222-2222-222222222222"), SecurityLevel = 2, GivenName = "Bob" },
            new Preferences { UserId = Guid.Parse("33333333-3333-3333-3333-333333333333"), SecurityLevel = 3, GivenName = "Charlie" },
            new Preferences { UserId = Guid.Parse("44444444-4444-4444-4444-444444444444"), SecurityLevel = 4, GivenName = "Diana" }
        };

        var filter = new PreferencesFilter_ArraySearchWithout
        {
            SecurityLevel = new[] { 1, 3 }
        };

        AssertParityPreferences(preferences, filter);
    }

    [Fact]
    public void Parity_IntArray_WithAttribute_MatchesRuntime()
    {
        var preferences = new[]
        {
            new Preferences { UserId = Guid.Parse("11111111-1111-1111-1111-111111111111"), SecurityLevel = 1, GivenName = "Alice" },
            new Preferences { UserId = Guid.Parse("22222222-2222-2222-2222-222222222222"), SecurityLevel = 2, GivenName = "Bob" },
            new Preferences { UserId = Guid.Parse("33333333-3333-3333-3333-333333333333"), SecurityLevel = 3, GivenName = "Charlie" },
            new Preferences { UserId = Guid.Parse("44444444-4444-4444-4444-444444444444"), SecurityLevel = 4, GivenName = "Diana" }
        };

        var filter = new PreferencesFilter_ArraySearchWith
        {
            SecurityLevel = new[] { 1, 3 }
        };

        AssertParityPreferences(preferences, filter);
    }

    [Fact]
    public void Parity_IntArray_WithAndWithoutAttribute_SameBehavior()
    {
        var preferences = new[]
        {
            new Preferences { UserId = Guid.Parse("11111111-1111-1111-1111-111111111111"), SecurityLevel = 5, GivenName = "Alice" },
            new Preferences { UserId = Guid.Parse("22222222-2222-2222-2222-222222222222"), SecurityLevel = 10, GivenName = "Bob" },
            new Preferences { UserId = Guid.Parse("33333333-3333-3333-3333-333333333333"), SecurityLevel = 15, GivenName = "Charlie" },
            new Preferences { UserId = Guid.Parse("44444444-4444-4444-4444-444444444444"), SecurityLevel = 20, GivenName = "Diana" }
        };

        var filterWithout = new PreferencesFilter_ArraySearchWithout { SecurityLevel = new[] { 10, 20 } };
        var filterWith = new PreferencesFilter_ArraySearchWith { SecurityLevel = new[] { 10, 20 } };

        var runtimeWithout = ExecutePreferences(() => filterWithout.ApplyFilterTo(preferences.AsQueryable()).ToList());
        var generatedWithout = ExecutePreferences(() => GeneratedFilterInvoker.ApplyFilter(preferences.AsQueryable(), filterWithout).ToList());

        var runtimeWith = ExecutePreferences(() => filterWith.ApplyFilterTo(preferences.AsQueryable()).ToList());
        var generatedWith = ExecutePreferences(() => GeneratedFilterInvoker.ApplyFilter(preferences.AsQueryable(), filterWith).ToList());

        Assert.Equal(runtimeWithout.Result.Select(x => x.UserId), generatedWithout.Result.Select(x => x.UserId));
        Assert.Equal(runtimeWith.Result.Select(x => x.UserId), generatedWith.Result.Select(x => x.UserId));
        Assert.Equal(runtimeWithout.Result.Select(x => x.UserId), runtimeWith.Result.Select(x => x.UserId));
    }

    [Fact]
    public void Parity_IntArray_EmptyArray_MatchesRuntime()
    {
        var preferences = new[]
        {
            new Preferences { UserId = Guid.Parse("11111111-1111-1111-1111-111111111111"), SecurityLevel = 1, GivenName = "Alice" }
        };

        var filterWithout = new PreferencesFilter_ArraySearchWithout { SecurityLevel = Array.Empty<int>() };
        var filterWith = new PreferencesFilter_ArraySearchWith { SecurityLevel = Array.Empty<int>() };

        var runtimeWithout = ExecutePreferences(() => filterWithout.ApplyFilterTo(preferences.AsQueryable()).ToList());
        var generatedWithout = ExecutePreferences(() => GeneratedFilterInvoker.ApplyFilter(preferences.AsQueryable(), filterWithout).ToList());

        var runtimeWith = ExecutePreferences(() => filterWith.ApplyFilterTo(preferences.AsQueryable()).ToList());
        var generatedWith = ExecutePreferences(() => GeneratedFilterInvoker.ApplyFilter(preferences.AsQueryable(), filterWith).ToList());

        Assert.Empty(runtimeWithout.Result);
        Assert.Empty(generatedWithout.Result);
        Assert.Empty(runtimeWith.Result);
        Assert.Empty(generatedWith.Result);
    }

    [Fact]
    public void Parity_IntArray_MultipleValues_MatchesAny()
    {
        var preferences = new[]
        {
            new Preferences { UserId = Guid.Parse("11111111-1111-1111-1111-111111111111"), SecurityLevel = 1, GivenName = "Alice" },
            new Preferences { UserId = Guid.Parse("22222222-2222-2222-2222-222222222222"), SecurityLevel = 5, GivenName = "Bob" },
            new Preferences { UserId = Guid.Parse("33333333-3333-3333-3333-333333333333"), SecurityLevel = 10, GivenName = "Charlie" },
            new Preferences { UserId = Guid.Parse("44444444-4444-4444-4444-444444444444"), SecurityLevel = 15, GivenName = "Diana" },
            new Preferences { UserId = Guid.Parse("55555555-5555-5555-5555-555555555555"), SecurityLevel = 20, GivenName = "Eve" }
        };

        var filter = new PreferencesFilter_ArraySearchWith { SecurityLevel = new[] { 5, 15, 25 } };

        AssertParityPreferences(preferences, filter);
    }

    #endregion

    #region Single Typed CompareTo(typeof(...)) Parity

    [Fact]
    public void Parity_SingleTypedCompareTo_ToLowerContains_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "clean code", Author = "John" },
            new Book { Id = 2, Title = "CLEAN CODE", Author = "Jane" },
            new Book { Id = 3, Title = "Clean Code", Author = "Bob" },
            new Book { Id = 4, Title = "Other", Author = "Alice" }
        };

        var filter = new BookFilter_TypeCompareTo { Search = "clean" };
        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_SingleTypedCompareTo_CaseInsensitive_ExactMatch()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Test Book", Author = "John" },
            new Book { Id = 2, Title = "TEST BOOK", Author = "Jane" },
            new Book { Id = 3, Title = "test book", Author = "Bob" },
            new Book { Id = 4, Title = "Test Books", Author = "Alice" }
        };

        var filter = new BookFilter_TypeCompareTo { Search = "test book" };
        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_SingleTypedCompareTo_PartialMatch_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "The Clean Code", Author = "John" },
            new Book { Id = 2, Title = "Clean Code", Author = "Jane" },
            new Book { Id = 3, Title = "Code Clean", Author = "Bob" },
            new Book { Id = 4, Title = "Other Book", Author = "Alice" }
        };

        var filter = new BookFilter_TypeCompareTo { Search = "clean" };
        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_SingleTypedCompareTo_NullValue_NoFilter()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Clean Code", Author = "John" },
            new Book { Id = 2, Title = "Other Book", Author = "Jane" }
        };

        var filter = new BookFilter_TypeCompareTo { Search = null };
        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_SingleTypedCompareTo_EmptyString_NoMatch()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Clean Code", Author = "John" },
            new Book { Id = 2, Title = "", Author = "Jane" },
            new Book { Id = 3, Title = "Other Book", Author = "Bob" }
        };

        var filter = new BookFilter_TypeCompareTo { Search = "" };
        AssertParity(books, filter);
    }

    #endregion

    #region StringFilter.NotContains + Compare Mode

    [Fact]
    public void Parity_StringFilter_NotContains_CompareOrdinal_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "CLEAN CODE", Author = "John" },
            new Book { Id = 2, Title = "Clean Code", Author = "Jane" },
            new Book { Id = 3, Title = "clean code", Author = "Bob" },
            new Book { Id = 4, Title = "Other Book", Author = "Alice" }
        };

        var filter = new BookFilter_StringFilter_Advanced
        {
            Title = new StringFilter
            {
                NotContains = "Clean",
                Compare = StringComparison.Ordinal
            }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_StringFilter_NotContains_CompareIgnoreCase_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "CLEAN CODE", Author = "John" },
            new Book { Id = 2, Title = "Clean Code", Author = "Jane" },
            new Book { Id = 3, Title = "clean code", Author = "Bob" },
            new Book { Id = 4, Title = "Other Book", Author = "Alice" }
        };

        var filter = new BookFilter_StringFilter_Advanced
        {
            Title = new StringFilter
            {
                NotContains = "clean",
                Compare = StringComparison.OrdinalIgnoreCase
            }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_StringFilter_NotContains_CompareInvariantCulture_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "café", Author = "Pierre" },
            new Book { Id = 2, Title = "cafe", Author = "John" },
            new Book { Id = 3, Title = "CAFÉ", Author = "Marie" },
            new Book { Id = 4, Title = "Other", Author = "Alice" }
        };

        var filter = new BookFilter_StringFilter_Advanced
        {
            Title = new StringFilter
            {
                NotContains = "caf",
                Compare = StringComparison.InvariantCultureIgnoreCase
            }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_StringFilter_NotContains_EmptyString_AllMatch()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A", Author = "John" },
            new Book { Id = 2, Title = "", Author = "Jane" },
            new Book { Id = 3, Title = null, Author = "Bob" }
        };

        var filter = new BookFilter_StringFilter_Advanced
        {
            Title = new StringFilter
            {
                NotContains = "",
                Compare = StringComparison.Ordinal
            }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_StringFilter_NotContains_NotExistingSubstring_AllMatch()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A", Author = "John" },
            new Book { Id = 2, Title = "Book B", Author = "Jane" },
            new Book { Id = 3, Title = "Book C", Author = "Bob" }
        };

        var filter = new BookFilter_StringFilter_Advanced
        {
            Title = new StringFilter
            {
                NotContains = "XYZ",
                Compare = StringComparison.Ordinal
            }
        };

        AssertParity(books, filter);
    }

    #endregion

    #region Strong Non-Nullable Range<T> Bound Parity

    [Fact]
    public void Parity_Range_NonNullable_MinAndMax_BothBoundsStrict()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book 1", Year = 1995, TotalPage = 150 },
            new Book { Id = 2, Title = "Book 2", Year = 2000, TotalPage = 250 },
            new Book { Id = 3, Title = "Book 3", Year = 2005, TotalPage = 350 },
            new Book { Id = 4, Title = "Book 4", Year = 2010, TotalPage = 450 },
            new Book { Id = 5, Title = "Book 5", Year = 1999, TotalPage = 199 },
            new Book { Id = 6, Title = "Book 6", Year = 2006, TotalPage = 301 }
        };

        var filter = new BookFilter_Range
        {
            Year = new Range<int> { Min = 2000, Max = 2005 }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_Range_NonNullable_BoundaryValues_Inclusive()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Below Min", Year = 1999, TotalPage = 100 },
            new Book { Id = 2, Title = "Exact Min", Year = 2000, TotalPage = 200 },
            new Book { Id = 3, Title = "In Range", Year = 2005, TotalPage = 250 },
            new Book { Id = 4, Title = "Exact Max", Year = 2010, TotalPage = 300 },
            new Book { Id = 5, Title = "Above Max", Year = 2011, TotalPage = 400 }
        };

        var filter = new BookFilter_Range
        {
            Year = new Range<int> { Min = 2000, Max = 2010 }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_Range_NonNullable_OnlyMin_StrictLowerBound()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book 1", Year = 1990, TotalPage = 100 },
            new Book { Id = 2, Title = "Book 2", Year = 2000, TotalPage = 200 },
            new Book { Id = 3, Title = "Book 3", Year = 2000, TotalPage = 205 },
            new Book { Id = 4, Title = "Book 4", Year = 1999, TotalPage = 150 }
        };

        var filter = new BookFilter_Range
        {
            Year = new Range<int> { Min = 2000 }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_Range_NonNullable_OnlyMax_StrictUpperBound()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book 1", Year = 2010, TotalPage = 300 },
            new Book { Id = 2, Title = "Book 2", Year = 2000, TotalPage = 200 },
            new Book { Id = 3, Title = "Book 3", Year = 2000, TotalPage = 205 },
            new Book { Id = 4, Title = "Book 4", Year = 2011, TotalPage = 400 }
        };

        var filter = new BookFilter_Range
        {
            Year = new Range<int> { Max = 2000 }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_Range_NonNullable_LargeDataset_PerformanceAndCorrectness()
    {
        var books = Enumerable.Range(1, 100).Select(i => new Book
        {
            Id = i,
            Title = $"Book {i}",
            Year = 1900 + i,
            TotalPage = 100 + i * 2
        }).ToArray();

        var filter = new BookFilter_Range
        {
            Year = new Range<int> { Min = 1950, Max = 1999 }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_Range_NonNullable_MultipleBooksSameBoundary()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book 1", Year = 2000, TotalPage = 100 },
            new Book { Id = 2, Title = "Book 2", Year = 2000, TotalPage = 200 },
            new Book { Id = 3, Title = "Book 3", Year = 2000, TotalPage = 300 },
            new Book { Id = 4, Title = "Book 4", Year = 2010, TotalPage = 400 },
            new Book { Id = 5, Title = "Book 5", Year = 2010, TotalPage = 500 }
        };

        var filter = new BookFilter_Range
        {
            Year = new Range<int> { Min = 2000, Max = 2010 }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_Range_NonNullable_AdjacentBoundaries()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book 1", Year = 1999, TotalPage = 100 },
            new Book { Id = 2, Title = "Book 2", Year = 2000, TotalPage = 200 },
            new Book { Id = 3, Title = "Book 3", Year = 2001, TotalPage = 300 },
            new Book { Id = 4, Title = "Book 4", Year = 2002, TotalPage = 400 },
            new Book { Id = 5, Title = "Book 5", Year = 2003, TotalPage = 500 }
        };

        var filter = new BookFilter_Range
        {
            Year = new Range<int> { Min = 2000, Max = 2002 }
        };

        AssertParity(books, filter);
    }

    #endregion

    #region Dotted-Path CompareTo Parity

    [Fact]
    public void Parity_DottedPathCompareTo_AuthorName_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book 1", Author = "Robert Martin", AuthorModel = new Author { Id = 1, Name = "Robert Martin" } },
            new Book { Id = 2, Title = "Book 2", Author = "John Doe", AuthorModel = new Author { Id = 2, Name = "John Doe" } },
            new Book { Id = 3, Title = "Book 3", Author = "Robert Smith", AuthorModel = new Author { Id = 3, Name = "Robert Smith" } },
            new Book { Id = 4, Title = "Book 4", Author = "Jane Wilson", AuthorModel = new Author { Id = 4, Name = "Jane Wilson" } },
            new Book { Id = 5, Title = "Book 5", Author = "Robert Johnson", AuthorModel = new Author { Id = 5, Name = "Robert Johnson" } }
        };

        var filter = new BookFilter_DottedPath { AuthorName = "Robert" };
        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_DottedPathCompareTo_CaseInsensitive_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book 1", AuthorModel = new Author { Id = 1, Name = "MARTIN" } },
            new Book { Id = 2, Title = "Book 2", AuthorModel = new Author { Id = 2, Name = "martin" } },
            new Book { Id = 3, Title = "Book 3", AuthorModel = new Author { Id = 3, Name = "Martin" } },
            new Book { Id = 4, Title = "Book 4", AuthorModel = new Author { Id = 4, Name = "Other" } }
        };

        var filter = new BookFilter_DottedPath { AuthorName = "martin" };
        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_DottedPathCompareTo_PartialMatch_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book 1", AuthorModel = new Author { Id = 1, Name = "The Clean Coder" } },
            new Book { Id = 2, Title = "Book 2", AuthorModel = new Author { Id = 2, Name = "Clean Code" } },
            new Book { Id = 3, Title = "Book 3", AuthorModel = new Author { Id = 3, Name = "Code Clean" } },
            new Book { Id = 4, Title = "Book 4", AuthorModel = new Author { Id = 4, Name = "Other Author" } }
        };

        var filter = new BookFilter_DottedPath { AuthorName = "clean" };
        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_DottedPathCompareTo_NullAuthorModel_NoMatch()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book 1", AuthorModel = new Author { Id = 1, Name = "Robert Martin" } },
            new Book { Id = 2, Title = "Book 2", AuthorModel = null },
            new Book { Id = 3, Title = "Book 3", AuthorModel = new Author { Id = 3, Name = "John Doe" } }
        };

        var filter = new BookFilter_DottedPath { AuthorName = "Martin" };
        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_DottedPathCompareTo_NullValue_NoFilter()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book 1", AuthorModel = new Author { Id = 1, Name = "Robert Martin" } },
            new Book { Id = 2, Title = "Book 2", AuthorModel = new Author { Id = 2, Name = "John Doe" } }
        };

        var filter = new BookFilter_DottedPath { AuthorName = null };
        AssertParity(books, filter);
    }

    #endregion

    #region CollectionFilterType.All Strict Parity

    [Fact]
    public void Parity_CollectionFilterType_All_NonEmptyCollection_StrictParity()
    {
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

        AssertParityAuthor(authors, filter);
    }

    [Fact]
    public void Parity_CollectionFilterType_All_EmptyCollection_StrictParity()
    {
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

        AssertParityAuthor(authors, filter);
    }

    [Fact]
    public void Parity_CollectionFilterType_All_MatchingItems_StrictParity()
    {
        var authors = new[]
        {
            new Author
            {
                Id = 1,
                Name = "All Match",
                Books = new List<Book>
                {
                    new Book { Title = "Clean Code", Year = 2000 },
                    new Book { Title = "Clean Architecture", Year = 2010 }
                }
            },
            new Author
            {
                Id = 2,
                Name = "Partial Match",
                Books = new List<Book>
                {
                    new Book { Title = "Clean Code", Year = 2000 },
                    new Book { Title = "Other Book", Year = 2015 }
                }
            },
            new Author
            {
                Id = 3,
                Name = "No Match",
                Books = new List<Book>
                {
                    new Book { Title = "Other Book", Year = 2015 },
                    new Book { Title = "Another Book", Year = 2020 }
                }
            }
        };

        var filter = new AuthorFilter_CollectionAll
        {
            Books = new BookNestedFilter
            {
                Title = new StringFilter { Contains = "Clean" }
            }
        };

        AssertParityAuthor(authors, filter);
    }

    [Fact]
    public void Parity_CollectionFilterType_All_NonMatchingItems_StrictParity()
    {
        var authors = new[]
        {
            new Author
            {
                Id = 1,
                Name = "All Clean",
                Books = new List<Book>
                {
                    new Book { Title = "Clean Code", Year = 2000 },
                    new Book { Title = "Clean Architecture", Year = 2010 }
                }
            },
            new Author
            {
                Id = 2,
                Name = "One Dirty",
                Books = new List<Book>
                {
                    new Book { Title = "Clean Code", Year = 2000 },
                    new Book { Title = "Dirty Code", Year = 2010 }
                }
            }
        };

        var filter = new AuthorFilter_CollectionAll
        {
            Books = new BookNestedFilter
            {
                Title = new StringFilter { Contains = "Clean" }
            }
        };

        AssertParityAuthor(authors, filter);
    }

    [Fact]
    public void Parity_CollectionFilterType_All_MultipleFilters_StrictParity()
    {
        var authors = new[]
        {
            new Author
            {
                Id = 1,
                Name = "All Match Both",
                Books = new List<Book>
                {
                    new Book { Title = "Clean Code", Year = 2000 },
                    new Book { Title = "Clean Design", Year = 2005 }
                }
            },
            new Author
            {
                Id = 2,
                Name = "Match One",
                Books = new List<Book>
                {
                    new Book { Title = "Clean Code", Year = 2000 },
                    new Book { Title = "Dirty Code", Year = 1995 }
                }
            }
        };

        var filter = new AuthorFilter_CollectionAll
        {
            Books = new BookNestedFilter
            {
                Title = new StringFilter { Contains = "Clean" },
                Year = new Range<int> { Min = 1999 }
            }
        };

        AssertParityAuthor(authors, filter);
    }

    [Fact]
    public void Parity_CollectionFilterType_All_NullBooksProperty_StrictParity()
    {
        var authors = new[]
        {
            new Author
            {
                Id = 1,
                Name = "Author With Null Books",
                Books = null
            },
            new Author
            {
                Id = 2,
                Name = "Author With Books",
                Books = new List<Book>
                {
                    new Book { Title = "Book 1", Year = 2000 }
                }
            }
        };

        var filter = new AuthorFilter_CollectionAll
        {
            Books = new BookNestedFilter
            {
                Year = new Range<int> { Min = 1999 }
            }
        };

        AssertParityAuthor(authors, filter);
    }

    #endregion

    #region Implicit Nested Mapping Parity

    [Fact]
    public void Parity_ImplicitNestedObject_ByPropertyName_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A", AuthorModel = new Author { Name = "Alice Smith" } },
            new Book { Id = 2, Title = "Book B", AuthorModel = new Author { Name = "Bob Jones" } },
            new Book { Id = 3, Title = "Book C", AuthorModel = new Author { Name = "Alice Johnson" } },
            new Book { Id = 4, Title = "Book D", AuthorModel = null }
        };

        var filter = new BookFilter_NestedObjectImplicit
        {
            AuthorModel = new AuthorFilterImplicit { Name = "Alice" }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_ImplicitNestedObject_WithMultipleProperties_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A", AuthorModel = new Author { Name = "Alice", Country = "USA", Age = 30 } },
            new Book { Id = 2, Title = "Book B", AuthorModel = new Author { Name = "Bob", Country = "UK", Age = 35 } },
            new Book { Id = 3, Title = "Book C", AuthorModel = new Author { Name = "Alice", Country = "Canada", Age = 25 } },
            new Book { Id = 4, Title = "Book D", AuthorModel = null }
        };

        var filter = new BookFilter_NestedObjectImplicit
        {
            AuthorModel = new AuthorFilterImplicit
            {
                Name = "Alice",
                Age = new Range<int> { Min = 28, Max = 35 }
            }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_ImplicitNestedObject_NullNestedObject_Ignored()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A", AuthorModel = null },
            new Book { Id = 2, Title = "Book B", AuthorModel = new Author { Name = "Alice" } },
            new Book { Id = 3, Title = "Book C", AuthorModel = null }
        };

        var filter = new BookFilter_NestedObjectImplicit
        {
            AuthorModel = new AuthorFilterImplicit { Name = "Alice" }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_ImplicitNestedObject_NullFilterValue_SkipsFilter()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A", AuthorModel = new Author { Name = "Alice" } },
            new Book { Id = 2, Title = "Book B", AuthorModel = new Author { Name = "Bob" } },
            new Book { Id = 3, Title = "Book C", AuthorModel = null }
        };

        var filter = new BookFilter_NestedObjectImplicit
        {
            AuthorModel = null
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_ImplicitNestedCollection_ByPropertyName_MatchesRuntime()
    {
        var authors = new[]
        {
            new Author
            {
                Id = 1,
                Name = "Author A",
                Books = new List<Book>
                {
                    new Book { Title = "Python Book", Year = 2020 },
                    new Book { Title = "Java Book", Year = 2019 }
                }
            },
            new Author
            {
                Id = 2,
                Name = "Author B",
                Books = new List<Book>
                {
                    new Book { Title = "C# Book", Year = 2021 },
                    new Book { Title = "JavaScript Book", Year = 2020 }
                }
            },
            new Author
            {
                Id = 3,
                Name = "Author C",
                Books = new List<Book>()
            }
        };

        var filter = new AuthorFilter_CollectionImplicit
        {
            Books = new BookFilterImplicit
            {
                Year = new Range<int> { Min = 2020 }
            }
        };

        AssertParityAuthor(authors, filter);
    }

    [Fact]
    public void Parity_ImplicitNestedCollection_WithTitleAndYear_MatchesRuntime()
    {
        var authors = new[]
        {
            new Author
            {
                Id = 1,
                Name = "Alice",
                Books = new List<Book>
                {
                    new Book { Title = "Clean Python", Year = 2020 },
                    new Book { Title = "Design Patterns", Year = 2015 }
                }
            },
            new Author
            {
                Id = 2,
                Name = "Bob",
                Books = new List<Book>
                {
                    new Book { Title = "Clean Code", Year = 2020 },
                    new Book { Title = "Refactoring", Year = 2018 }
                }
            },
            new Author
            {
                Id = 3,
                Name = "Charlie",
                Books = new List<Book>()
            }
        };

        var filter = new AuthorFilter_CollectionImplicit
        {
            Name = "Alice",
            Books = new BookFilterImplicit
            {
                Title = "Clean",
                Year = new Range<int> { Min = 2019, Max = 2021 }
            }
        };

        AssertParityAuthor(authors, filter);
    }

    [Fact]
    public void Parity_ImplicitNestedCollection_EmptyCollection_NoMatches()
    {
        var authors = new[]
        {
            new Author
            {
                Id = 1,
                Name = "Author A",
                Books = new List<Book>()
            },
            new Author
            {
                Id = 2,
                Name = "Author B",
                Books = new List<Book>
                {
                    new Book { Title = "Book 1", Year = 2020 }
                }
            }
        };

        var filter = new AuthorFilter_CollectionImplicit
        {
            Books = new BookFilterImplicit
            {
                Year = new Range<int> { Min = 2020 }
            }
        };

        AssertParityAuthor(authors, filter);
    }

    [Fact]
    public void Parity_ImplicitNestedCollection_NullBooksProperty_Ignored()
    {
        var authors = new[]
        {
            new Author
            {
                Id = 1,
                Name = "Author With Null Books",
                Books = null
            },
            new Author
            {
                Id = 2,
                Name = "Author With Books",
                Books = new List<Book>
                {
                    new Book { Title = "Book 1", Year = 2020 }
                }
            }
        };

        var filter = new AuthorFilter_CollectionImplicit
        {
            Books = new BookFilterImplicit
            {
                Year = new Range<int> { Min = 2020 }
            }
        };

        AssertParityAuthor(authors, filter);
    }

    #endregion

    #region Deep-Edge Nested Collection Null/Empty Parity

    [Fact]
    public void Parity_DeepNested_NullOuterCollection_MatchesRuntime()
    {
        var publishers = new[]
        {
            new Publisher
            {
                Id = 1,
                Name = "Publisher A",
                Authors = new List<Author>
                {
                    new Author
                    {
                        Id = 1,
                        Name = "Author A",
                        Books = new List<Book>
                        {
                            new Book { Title = "Book 1", Year = 2000 }
                        }
                    }
                }
            },
            new Publisher
            {
                Id = 2,
                Name = "Publisher B",
                Authors = null
            },
            new Publisher
            {
                Id = 3,
                Name = "Publisher C",
                Authors = new List<Author>
                {
                    new Author
                    {
                        Id = 2,
                        Name = "Author C",
                        Books = new List<Book>
                        {
                            new Book { Title = "Book 2", Year = 2010 }
                        }
                    }
                }
            }
        };

        var filter = new PublisherFilter_NestedCollection
        {
            Authors = new AuthorNestedFilter
            {
                Books = new BookNestedFilter
                {
                    Year = new Range<int> { Min = 2005 }
                }
            }
        };

        AssertParityPublisher(publishers, filter);
    }

    [Fact]
    public void Parity_DeepNested_NullInnerCollection_MatchesRuntime()
    {
        var publishers = new[]
        {
            new Publisher
            {
                Id = 1,
                Name = "Publisher A",
                Authors = new List<Author>
                {
                    new Author
                    {
                        Id = 1,
                        Name = "Author A",
                        Books = new List<Book>
                        {
                            new Book { Title = "Book 1", Year = 2000 }
                        }
                    }
                }
            },
            new Publisher
            {
                Id = 2,
                Name = "Publisher B",
                Authors = new List<Author>
                {
                    new Author
                    {
                        Id = 2,
                        Name = "Author B",
                        Books = null
                    }
                }
            },
            new Publisher
            {
                Id = 3,
                Name = "Publisher C",
                Authors = new List<Author>
                {
                    new Author
                    {
                        Id = 3,
                        Name = "Author C",
                        Books = new List<Book>
                        {
                            new Book { Title = "Book 2", Year = 2010 }
                        }
                    }
                }
            }
        };

        var filter = new PublisherFilter_NestedCollection
        {
            Authors = new AuthorNestedFilter
            {
                Books = new BookNestedFilter
                {
                    Year = new Range<int> { Min = 2005 }
                }
            }
        };

        AssertParityPublisher(publishers, filter);
    }

    [Fact]
    public void Parity_DeepNested_NullBothOuterAndInnerCollections_MatchesRuntime()
    {
        var publishers = new[]
        {
            new Publisher
            {
                Id = 1,
                Name = "Publisher A",
                Authors = new List<Author>
                {
                    new Author
                    {
                        Id = 1,
                        Name = "Author A",
                        Books = new List<Book>
                        {
                            new Book { Title = "Book 1", Year = 2000 }
                        }
                    }
                }
            },
            new Publisher
            {
                Id = 2,
                Name = "Publisher B",
                Authors = new List<Author>
                {
                    new Author
                    {
                        Id = 2,
                        Name = "Author B",
                        Books = null
                    },
                    new Author
                    {
                        Id = 3,
                        Name = "Author C",
                        Books = new List<Book>
                        {
                            new Book { Title = "Book 2", Year = 2010 }
                        }
                    }
                }
            },
            new Publisher
            {
                Id = 3,
                Name = "Publisher C",
                Authors = null
            }
        };

        var filter = new PublisherFilter_NestedCollection
        {
            Authors = new AuthorNestedFilter
            {
                Books = new BookNestedFilter
                {
                    Year = new Range<int> { Min = 2005 }
                }
            }
        };

        AssertParityPublisher(publishers, filter);
    }

    [Fact]
    public void Parity_DeepNested_EmptyFilterObject_Any_MatchesRuntime()
    {
        var publishers = new[]
        {
            new Publisher
            {
                Id = 1,
                Name = "Publisher A",
                Authors = new List<Author>
                {
                    new Author
                    {
                        Id = 1,
                        Name = "Author A",
                        Books = new List<Book>
                        {
                            new Book { Title = "Book 1", Year = 2000 }
                        }
                    }
                }
            },
            new Publisher
            {
                Id = 2,
                Name = "Publisher B",
                Authors = new List<Author>
                {
                    new Author
                    {
                        Id = 2,
                        Name = "Author B",
                        Books = new List<Book>()
                    }
                }
            },
            new Publisher
            {
                Id = 3,
                Name = "Publisher C",
                Authors = new List<Author>
                {
                    new Author
                    {
                        Id = 3,
                        Name = "Author C",
                        Books = new List<Book>
                        {
                            new Book { Title = "Book 2", Year = 2010 }
                        }
                    }
                }
            }
        };

        // Empty nested filter object for Any - should match when nested collection has any items
        var filter = new PublisherFilter_NestedCollection
        {
            Authors = new AuthorNestedFilter
            {
                Books = new BookNestedFilter()
            }
        };

        AssertParityPublisher(publishers, filter);
    }

    [Fact]
    public void Parity_DeepNested_EmptyFilterObject_All_MatchesRuntime()
    {
        var publishers = new[]
        {
            new Publisher
            {
                Id = 1,
                Name = "Publisher A",
                Authors = new List<Author>
                {
                    new Author
                    {
                        Id = 1,
                        Name = "Author A",
                        Books = new List<Book>
                        {
                            new Book { Title = "Book 1", Year = 2000 }
                        }
                    }
                }
            },
            new Publisher
            {
                Id = 2,
                Name = "Publisher B",
                Authors = new List<Author>
                {
                    new Author
                    {
                        Id = 2,
                        Name = "Author B",
                        Books = new List<Book>()
                    }
                }
            },
            new Publisher
            {
                Id = 3,
                Name = "Publisher C",
                Authors = new List<Author>
                {
                    new Author
                    {
                        Id = 3,
                        Name = "Author C",
                        Books = new List<Book>
                        {
                            new Book { Title = "Book 2", Year = 2010 }
                        }
                    }
                }
            }
        };

        // Empty nested filter object for All - should match when nested collection has all items matching
        var filter = new PublisherFilter_NestedCollectionAll
        {
            Authors = new AuthorNestedFilter_CollectionAll
            {
                Books = new BookNestedFilter()
            }
        };

        AssertParityPublisher(publishers, filter);
    }

    [Fact]
    public void Parity_DeepNested_EmptyFilterObject_All_MultipleAuthors_MatchesRuntime()
    {
        var publishers = new[]
        {
            new Publisher
            {
                Id = 1,
                Name = "All Authors Have Books",
                Authors = new List<Author>
                {
                    new Author
                    {
                        Id = 1,
                        Name = "Author A",
                        Books = new List<Book>
                        {
                            new Book { Title = "Book 1", Year = 2000 },
                            new Book { Title = "Book 2", Year = 2005 }
                        }
                    },
                    new Author
                    {
                        Id = 2,
                        Name = "Author B",
                        Books = new List<Book>
                        {
                            new Book { Title = "Book 3", Year = 2010 }
                        }
                    }
                }
            },
            new Publisher
            {
                Id = 2,
                Name = "One Author Has Empty Books",
                Authors = new List<Author>
                {
                    new Author
                    {
                        Id = 3,
                        Name = "Author C",
                        Books = new List<Book>
                        {
                            new Book { Title = "Book 4", Year = 2015 }
                        }
                    },
                    new Author
                    {
                        Id = 4,
                        Name = "Author D",
                        Books = new List<Book>()
                    }
                }
            }
        };

        var filter = new PublisherFilter_NestedCollectionAll
        {
            Authors = new AuthorNestedFilter_CollectionAll
            {
                Books = new BookNestedFilter()
            }
        };

        AssertParityPublisher(publishers, filter);
    }

    [Fact]
    public void Parity_DeepNested_MultipleLevels_CombinedFilters_MatchesRuntime()
    {
        var publishers = new[]
        {
            new Publisher
            {
                Id = 1,
                Name = "Tech Publisher",
                Authors = new List<Author>
                {
                    new Author
                    {
                        Id = 1,
                        Name = "Alice",
                        Books = new List<Book>
                        {
                            new Book { Title = "Clean Code", Year = 2000 },
                            new Book { Title = "Refactoring", Year = 2005 }
                        }
                    },
                    new Author
                    {
                        Id = 2,
                        Name = "Bob",
                        Books = new List<Book>
                        {
                            new Book { Title = "Design Patterns", Year = 1995 },
                            new Book { Title = "Clean Architecture", Year = 2010 }
                        }
                    }
                }
            },
            new Publisher
            {
                Id = 2,
                Name = "Other Publisher",
                Authors = new List<Author>
                {
                    new Author
                    {
                        Id = 3,
                        Name = "Charlie",
                        Books = new List<Book>
                        {
                            new Book { Title = "Fiction Book", Year = 2015 }
                        }
                    }
                }
            }
        };

        var filter = new PublisherFilter_NestedCollection
        {
            Authors = new AuthorNestedFilter
            {
                Books = new BookNestedFilter
                {
                    Title = new StringFilter { Contains = "Clean" }
                }
            }
        };

        AssertParityPublisher(publishers, filter);
    }

    #endregion

    private static void AssertParity<TFilter>(IEnumerable<Book> books, TFilter filter)
        where TFilter : FilterBase
    {
        var runtime = Execute(() => filter.ApplyFilterTo(books.AsQueryable()).ToList());
        var generated = Execute(() => GeneratedFilterInvoker.ApplyFilter(books.AsQueryable(), filter).ToList());

        Assert.Equal(runtime.Exception?.GetType(), generated.Exception?.GetType());

        if (runtime.Exception != null)
        {
            Assert.Equal(runtime.Exception.Message, generated.Exception.Message);
            return;
        }

        Assert.Equal(runtime.Result.Select(x => x.Id), generated.Result.Select(x => x.Id));
    }

    private static void AssertParityAuthor<TFilter>(IEnumerable<Author> authors, TFilter filter)
        where TFilter : FilterBase
    {
        var runtime = ExecuteAuthor(() => filter.ApplyFilterTo(authors.AsQueryable()).ToList());
        var generated = ExecuteAuthor(() => GeneratedFilterInvoker.ApplyFilter(authors.AsQueryable(), filter).ToList());

        Assert.Equal(runtime.Exception?.GetType(), generated.Exception?.GetType());

        if (runtime.Exception != null)
        {
            Assert.Equal(runtime.Exception.Message, generated.Exception.Message);
            return;
        }

        Assert.Equal(runtime.Result.Select(x => x.Id), generated.Result.Select(x => x.Id));
    }

    private static void AssertParityPreferences<TFilter>(IEnumerable<Preferences> preferences, TFilter filter)
        where TFilter : FilterBase
    {
        var runtime = ExecutePreferences(() => filter.ApplyFilterTo(preferences.AsQueryable()).ToList());
        var generated = ExecutePreferences(() => GeneratedFilterInvoker.ApplyFilter(preferences.AsQueryable(), filter).ToList());

        Assert.Equal(runtime.Exception?.GetType(), generated.Exception?.GetType());

        if (runtime.Exception != null)
        {
            Assert.Equal(runtime.Exception.Message, generated.Exception.Message);
            return;
        }

        Assert.Equal(runtime.Result.Select(x => x.UserId), generated.Result.Select(x => x.UserId));
    }

    private static void AssertParityPublisher<TFilter>(IEnumerable<Publisher> publishers, TFilter filter)
        where TFilter : FilterBase
    {
        var runtime = ExecutePublisher(() => filter.ApplyFilterTo(publishers.AsQueryable()).ToList());
        var generated = ExecutePublisher(() => GeneratedFilterInvoker.ApplyFilter(publishers.AsQueryable(), filter).ToList());

        Assert.Equal(runtime.Exception?.GetType(), generated.Exception?.GetType());

        if (runtime.Exception != null)
        {
            Assert.Equal(runtime.Exception.Message, generated.Exception.Message);
            return;
        }

        Assert.Equal(runtime.Result.Select(x => x.Id), generated.Result.Select(x => x.Id));
    }

    private static ExecutionResult Execute(Func<List<Book>> run)
    {
        try
        {
            return new ExecutionResult { Result = run() };
        }
        catch (Exception ex)
        {
            return new ExecutionResult { Exception = ex };
        }
    }

    private static ExecutionResultAuthor ExecuteAuthor(Func<List<Author>> run)
    {
        try
        {
            return new ExecutionResultAuthor { Result = run() };
        }
        catch (Exception ex)
        {
            return new ExecutionResultAuthor { Exception = ex };
        }
    }

    private static ExecutionResultPreferences ExecutePreferences(Func<List<Preferences>> run)
    {
        try
        {
            return new ExecutionResultPreferences { Result = run() };
        }
        catch (Exception ex)
        {
            return new ExecutionResultPreferences { Exception = ex };
        }
    }

    private static ExecutionResultPublisher ExecutePublisher(Func<List<Publisher>> run)
    {
        try
        {
            return new ExecutionResultPublisher { Result = run() };
        }
        catch (Exception ex)
        {
            return new ExecutionResultPublisher { Exception = ex };
        }
    }

    private sealed class ExecutionResult
    {
        public List<Book> Result { get; set; } = new List<Book>();
        public Exception Exception { get; set; }
    }

    private sealed class ExecutionResultAuthor
    {
        public List<Author> Result { get; set; } = new List<Author>();
        public Exception Exception { get; set; }
    }

    private sealed class ExecutionResultPreferences
    {
        public List<Preferences> Result { get; set; } = new List<Preferences>();
        public Exception Exception { get; set; }
    }

    private sealed class ExecutionResultPublisher
    {
        public List<Publisher> Result { get; set; } = new List<Publisher>();
        public Exception Exception { get; set; }
    }
}
