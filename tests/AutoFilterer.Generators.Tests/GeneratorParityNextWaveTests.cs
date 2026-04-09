using AutoFilterer.Enums;
using AutoFilterer.Extensions;
using AutoFilterer.Generators.Tests.Environment.Dtos;
using AutoFilterer.Generators.Tests.Environment.Models;
using AutoFilterer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AutoFilterer.Generators.Tests;

public class GeneratorParityNextWaveTests
{
    #region CompareTo Multiple Targets AND Semantics

    [Fact]
    public void Parity_CompareToMultipleTargets_AND_Semantics_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Clean Code", Author = "Robert Martin" },
            new Book { Id = 2, Title = "Clean Architecture", Author = "Robert Martin" },
            new Book { Id = 3, Title = "Code Complete", Author = "Steve McConnell" },
            new Book { Id = 4, Title = "Refactoring", Author = "Martin Fowler" }
        };

        var filter = new BookFilter_MultiplePropertyAnd { Query = "Martin" };
        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_CompareToMultipleTargets_AND_NoMatchWhenPartial()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Martin's Book", Author = "Other Author" },
            new Book { Id = 2, Title = "Book Title", Author = "Martin Smith" },
            new Book { Id = 3, Title = "Martin's Work", Author = "Martin Jones" }
        };

        var filter = new BookFilter_MultiplePropertyAnd { Query = "Martin" };
        AssertParity(books, filter);
    }

    #endregion

    #region Multiple Typed CompareTo Attributes OR/AND

    [Fact]
    public void Parity_MultipleTypedCompareTo_OR_Semantics_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "clean code", Author = "John Doe" },
            new Book { Id = 2, Title = "Other", Author = "CLEAN Smith" },
            new Book { Id = 3, Title = "None", Author = "Bob Jones" }
        };

        var filter = new BookFilter_MultipleTypeCompareTo { Search = "clean" };
        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_MultipleTypedCompareTo_AND_Semantics_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "testing book", Author = "John testing" },
            new Book { Id = 2, Title = "testing book", Author = "John other" },
            new Book { Id = 3, Title = "other book", Author = "John testing" }
        };

        var filter = new BookFilter_MultipleTypeCompareToAnd { Search = "testing" };
        AssertParity(books, filter);
    }

    #endregion

    #region Invalid CompareTo Metadata (IgnoreExceptions=false)

    [Fact]
    public void Parity_InvalidCompareTo_ThrowsWhenIgnoreExceptionsFalse()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Test", Author = "John" }
        };

        var filter = new BookFilter_InvalidTarget { Search = "value" };
        AssertParity(books, filter);
    }

    #endregion

    #region ToLowerEqualsComparison Semantics

    [Fact]
    public void Parity_ToLowerEquals_CaseInsensitiveExactMatch()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Clean Code", Author = "John" },
            new Book { Id = 2, Title = "CLEAN CODE", Author = "Jane" },
            new Book { Id = 3, Title = "clean code", Author = "Bob" },
            new Book { Id = 4, Title = "Clean Codes", Author = "Alice" },
            new Book { Id = 5, Title = "The Clean Code", Author = "Tom" }
        };

        var filter = new BookFilter_ToLowerEquals { Title = "clean code" };
        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_ToLowerEquals_SkipsFilterWhenValueNull()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A", Author = "John" },
            new Book { Id = 2, Title = "Book B", Author = "Jane" },
            new Book { Id = 3, Title = "Book C", Author = "Bob" }
        };

        var filter = new BookFilter_ToLowerEquals { Title = null };
        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_ToLowerEquals_SkipsFilterWhenValueEmptyString()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "", Author = "John" },
            new Book { Id = 2, Title = null, Author = "Jane" },
            new Book { Id = 3, Title = "Book C", Author = "Bob" }
        };

        var filter = new BookFilter_ToLowerEquals { Title = "" };
        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_ToLowerEquals_SpecialCharactersMatched()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Test-Book_123", Author = "John" },
            new Book { Id = 2, Title = "test-book_123", Author = "Jane" },
            new Book { Id = 3, Title = "TEST-BOOK_123", Author = "Bob" },
            new Book { Id = 4, Title = "TestBook123", Author = "Alice" }
        };

        var filter = new BookFilter_ToLowerEquals { Title = "test-book_123" };
        AssertParity(books, filter);
    }

    #endregion

    #region Top-Level FilterBase.CombineWith Across Scalar Properties

    [Fact]
    public void Parity_FilterBaseCombineWith_OR_AcrossScalarProperties()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Test", Author = "Other" },
            new Book { Id = 2, Title = "Other", Author = "Test" },
            new Book { Id = 3, Title = "Test", Author = "Test" },
            new Book { Id = 4, Title = "None", Author = "None" }
        };

        var filter = new BookFilter_ScalarCombineWith
        {
            Title = "Test",
            Author = "Test",
            CombineWith = CombineType.Or
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_FilterBaseCombineWith_AND_AcrossScalarProperties()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Test", Author = "Other" },
            new Book { Id = 2, Title = "Other", Author = "Test" },
            new Book { Id = 3, Title = "Test", Author = "Test" },
            new Book { Id = 4, Title = "None", Author = "None" }
        };

        var filter = new BookFilter_ScalarCombineWith
        {
            Title = "Test",
            Author = "Test",
            CombineWith = CombineType.And
        };

        AssertParity(books, filter);
    }

    #endregion

    #region Orderable Edge Behavior

    [Fact]
    public void Parity_Orderable_NullSort_DoesNotThrow()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A", Author = "John", TotalPage = 100 },
            new Book { Id = 2, Title = "Book B", Author = "Jane", TotalPage = 200 }
        };

        var filter = new BookFilter_OrderableEdge
        {
            Sort = null,
            SortBy = Sorting.Ascending
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_Orderable_DottedPathSort_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A", AuthorModel = new Author { Name = "Zoe" } },
            new Book { Id = 2, Title = "Book B", AuthorModel = new Author { Name = "Alice" } },
            new Book { Id = 3, Title = "Book C", AuthorModel = new Author { Name = "Bob" } }
        };

        var filter = new BookFilter_OrderableEdge
        {
            Sort = "AuthorModel.Name",
            SortBy = Sorting.Ascending
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_Orderable_DottedPathSort_Descending_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A", AuthorModel = new Author { Name = "Zoe" } },
            new Book { Id = 2, Title = "Book B", AuthorModel = new Author { Name = "Alice" } },
            new Book { Id = 3, Title = "Book C", AuthorModel = new Author { Name = "Bob" } }
        };

        var filter = new BookFilter_OrderableEdge
        {
            Sort = "AuthorModel.Name",
            SortBy = Sorting.Descending
        };

        AssertParity(books, filter);
    }

    #endregion

    #region Nested Non-Collection Object Filter Behavior

    [Fact]
    public void Parity_NestedNonCollectionObject_FilterMatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A", AuthorModel = new Author { Name = "Alice Smith" } },
            new Book { Id = 2, Title = "Book B", AuthorModel = new Author { Name = "Bob Jones" } },
            new Book { Id = 3, Title = "Book C", AuthorModel = new Author { Name = "Alice Johnson" } }
        };

        var filter = new BookFilter_NestedObject
        {
            AuthorFilter = new AuthorModelFilter { Name = "Alice" }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_NestedNonCollectionObject_NullNestedObject_Ignored()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A", AuthorModel = null },
            new Book { Id = 2, Title = "Book B", AuthorModel = new Author { Name = "Alice" } },
            new Book { Id = 3, Title = "Book C", AuthorModel = null }
        };

        var filter = new BookFilter_NestedObject
        {
            AuthorFilter = new AuthorModelFilter { Name = "Alice" }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_NestedNonCollectionObject_NullFilterValue_SkipsFilter()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A", AuthorModel = new Author { Name = "Alice" } },
            new Book { Id = 2, Title = "Book B", AuthorModel = new Author { Name = "Bob" } },
            new Book { Id = 3, Title = "Book C", AuthorModel = null }
        };

        var filter = new BookFilter_NestedObject
        {
            AuthorFilter = null
        };

        AssertParity(books, filter);
    }

    #endregion

    #region Nullable Range<T> Min+Max Behavior

    [Fact]
    public void Parity_NullableRange_MinAndMax_FilterBothBounds()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A", Views = 100 },
            new Book { Id = 2, Title = "Book B", Views = 200 },
            new Book { Id = 3, Title = "Book C", Views = null },
            new Book { Id = 4, Title = "Book D", Views = 300 },
            new Book { Id = 5, Title = "Book E", Views = 250 }
        };

        var filter = new BookFilter_Range
        {
            Views = new Range<int> { Min = 150, Max = 250 }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_NullableRange_OnlyMin_OnlyLowerBound()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A", Views = 100 },
            new Book { Id = 2, Title = "Book B", Views = 200 },
            new Book { Id = 3, Title = "Book C", Views = null },
            new Book { Id = 4, Title = "Book D", Views = 150 }
        };

        var filter = new BookFilter_Range
        {
            Views = new Range<int> { Min = 150 }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_NullableRange_OnlyMax_OnlyUpperBound()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A", Views = 100 },
            new Book { Id = 2, Title = "Book B", Views = 200 },
            new Book { Id = 3, Title = "Book C", Views = null },
            new Book { Id = 4, Title = "Book D", Views = 150 }
        };

        var filter = new BookFilter_Range
        {
            Views = new Range<int> { Max = 150 }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_NullableRange_NullMinAndMax_Ignored()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A", Views = 100 },
            new Book { Id = 2, Title = "Book B", Views = null },
            new Book { Id = 3, Title = "Book C", Views = 300 }
        };

        var filter = new BookFilter_Range
        {
            Views = new Range<int> { Min = null, Max = null }
        };

        AssertParity(books, filter);
    }

    #endregion

    private static void AssertParity<TFilter>(IEnumerable<Book> books, TFilter filter)
        where TFilter : FilterBase
    {
        var runtime = Execute(() => filter.ApplyFilterTo(books.AsQueryable()).ToList());
        var generated = Execute(() => books.AsQueryable().ApplyFilter(filter).ToList());

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

    private sealed class ExecutionResult
    {
        public List<Book> Result { get; set; } = new List<Book>();

        public Exception Exception { get; set; }
    }
}
