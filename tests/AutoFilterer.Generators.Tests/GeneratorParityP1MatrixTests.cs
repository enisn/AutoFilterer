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

/// <summary>
/// Strict parity tests for remaining P1 scenario groups:
/// - Full OperatorFilter matrix with CombineWith
/// - Full StringFilter matrix
/// - CollectionFilterType.Any and nested chains
/// - Multi-target Range CompareTo
/// - Invalid CompareTo filterable type behavior
/// </summary>
public class GeneratorParityP1MatrixTests
{
    #region OperatorFilter Full Matrix

    [Fact]
    public void Parity_OperatorFilter_Eq_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A", TotalPage = 100 },
            new Book { Id = 2, Title = "Book B", TotalPage = 200 },
            new Book { Id = 3, Title = "Book C", TotalPage = 100 }
        };

        var filter = new BookFilter_OperatorFilter_Matrix
        {
            TotalPage = new OperatorFilter<int> { Eq = 100 }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_OperatorFilter_Not_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A", TotalPage = 100 },
            new Book { Id = 2, Title = "Book B", TotalPage = 200 },
            new Book { Id = 3, Title = "Book C", TotalPage = 300 }
        };

        var filter = new BookFilter_OperatorFilter_Matrix
        {
            TotalPage = new OperatorFilter<int> { Not = 200 }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_OperatorFilter_Gt_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A", TotalPage = 100 },
            new Book { Id = 2, Title = "Book B", TotalPage = 200 },
            new Book { Id = 3, Title = "Book C", TotalPage = 300 }
        };

        var filter = new BookFilter_OperatorFilter_Matrix
        {
            TotalPage = new OperatorFilter<int> { Gt = 150 }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_OperatorFilter_Gte_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A", TotalPage = 100 },
            new Book { Id = 2, Title = "Book B", TotalPage = 200 },
            new Book { Id = 3, Title = "Book C", TotalPage = 300 }
        };

        var filter = new BookFilter_OperatorFilter_Matrix
        {
            TotalPage = new OperatorFilter<int> { Gte = 200 }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_OperatorFilter_Lt_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A", TotalPage = 100 },
            new Book { Id = 2, Title = "Book B", TotalPage = 200 },
            new Book { Id = 3, Title = "Book C", TotalPage = 300 }
        };

        var filter = new BookFilter_OperatorFilter_Matrix
        {
            TotalPage = new OperatorFilter<int> { Lt = 250 }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_OperatorFilter_Lte_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A", TotalPage = 100 },
            new Book { Id = 2, Title = "Book B", TotalPage = 200 },
            new Book { Id = 3, Title = "Book C", TotalPage = 300 }
        };

        var filter = new BookFilter_OperatorFilter_Matrix
        {
            TotalPage = new OperatorFilter<int> { Lte = 200 }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_OperatorFilter_MultipleOperators_OR_Semantics()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A", TotalPage = 100 },
            new Book { Id = 2, Title = "Book B", TotalPage = 200 },
            new Book { Id = 3, Title = "Book C", TotalPage = 300 },
            new Book { Id = 4, Title = "Book D", TotalPage = 150 }
        };

        var filter = new BookFilter_OperatorFilter_Matrix
        {
            TotalPage = new OperatorFilter<int>
            {
                Gt = 150,
                Eq = 100,
                CombineWith = CombineType.Or
            }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_OperatorFilter_NullableProperty_IsNull()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A", Views = 100 },
            new Book { Id = 2, Title = "Book B", Views = null },
            new Book { Id = 3, Title = "Book C", Views = 200 }
        };

        var filter = new BookFilter_OperatorFilter_Matrix
        {
            Views = new OperatorFilter<int> { IsNull = true }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_OperatorFilter_NullableProperty_IsNotNull()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A", Views = 100 },
            new Book { Id = 2, Title = "Book B", Views = null },
            new Book { Id = 3, Title = "Book C", Views = 200 }
        };

        var filter = new BookFilter_OperatorFilter_Matrix
        {
            Views = new OperatorFilter<int> { IsNotNull = true }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_OperatorFilter_NullableProperty_WithComparison()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A", Views = 100 },
            new Book { Id = 2, Title = "Book B", Views = null },
            new Book { Id = 3, Title = "Book C", Views = 200 },
            new Book { Id = 4, Title = "Book D", Views = 150 }
        };

        var filter = new BookFilter_OperatorFilter_Matrix
        {
            Views = new OperatorFilter<int> { Gte = 150 }
        };

        AssertParity(books, filter);
    }

    #endregion

    #region StringFilter Full Matrix

    [Fact]
    public void Parity_StringFilter_Contains_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Clean Code" },
            new Book { Id = 2, Title = "Code Complete" },
            new Book { Id = 3, Title = "Refactoring" },
            new Book { Id = 4, Title = "Coding Patterns" }
        };

        var filter = new BookFilter_StringFilter_Matrix
        {
            Title = new StringFilter { Contains = "Code" }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_StringFilter_NotContains_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Clean Code" },
            new Book { Id = 2, Title = "Code Complete" },
            new Book { Id = 3, Title = "Refactoring" },
            new Book { Id = 4, Title = "Coding Patterns" }
        };

        var filter = new BookFilter_StringFilter_Matrix
        {
            Title = new StringFilter { NotContains = "Code" }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_StringFilter_StartsWith_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Clean Code" },
            new Book { Id = 2, Title = "Code Complete" },
            new Book { Id = 3, Title = "The Clean Code" },
            new Book { Id = 4, Title = "Refactoring" }
        };

        var filter = new BookFilter_StringFilter_Matrix
        {
            Title = new StringFilter { StartsWith = "Clean" }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_StringFilter_EndsWith_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Clean Code" },
            new Book { Id = 2, Title = "Code Complete" },
            new Book { Id = 3, Title = "The Clean Code" },
            new Book { Id = 4, Title = "Refactoring" }
        };

        var filter = new BookFilter_StringFilter_Matrix
        {
            Title = new StringFilter { EndsWith = "Code" }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_StringFilter_IsNull_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A" },
            new Book { Id = 2, Title = null },
            new Book { Id = 3, Title = "Book C" },
            new Book { Id = 4, Title = null }
        };

        var filter = new BookFilter_StringFilter_Matrix
        {
            Title = new StringFilter { IsNull = true }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_StringFilter_IsNotNull_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A" },
            new Book { Id = 2, Title = null },
            new Book { Id = 3, Title = "Book C" },
            new Book { Id = 4, Title = null }
        };

        var filter = new BookFilter_StringFilter_Matrix
        {
            Title = new StringFilter { IsNotNull = true }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_StringFilter_IsEmpty_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A" },
            new Book { Id = 2, Title = "" },
            new Book { Id = 3, Title = "Book C" },
            new Book { Id = 4, Title = "" }
        };

        var filter = new BookFilter_StringFilter_Matrix
        {
            Title = new StringFilter { IsEmpty = true }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_StringFilter_IsNotEmpty_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A" },
            new Book { Id = 2, Title = "" },
            new Book { Id = 3, Title = "Book C" },
            new Book { Id = 4, Title = "" }
        };

        var filter = new BookFilter_StringFilter_Matrix
        {
            Title = new StringFilter { IsNotEmpty = true }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_StringFilter_Contains_CaseSensitive()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "CLEAN CODE" },
            new Book { Id = 2, Title = "Clean Code" },
            new Book { Id = 3, Title = "clean code" },
            new Book { Id = 4, Title = "Clean Code" }
        };

        var filter = new BookFilter_StringFilter_Matrix
        {
            Title = new StringFilter
            {
                Contains = "Clean",
                Compare = StringComparison.Ordinal
            }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_StringFilter_MultipleOptions_OR_Semantics()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Clean Code" },
            new Book { Id = 2, Title = "Patterns" },
            new Book { Id = 3, Title = "" },
            new Book { Id = 4, Title = "Refactoring" },
            new Book { Id = 5, Title = "Clean" }
        };

        var filter = new BookFilter_StringFilter_Matrix
        {
            Title = new StringFilter
            {
                StartsWith = "Clean",
                IsEmpty = true,
                CombineWith = CombineType.Or
            }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_StringFilter_Eq_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Clean Code" },
            new Book { Id = 2, Title = "Code Complete" },
            new Book { Id = 3, Title = "Clean Code" },
            new Book { Id = 4, Title = "Refactoring" }
        };

        var filter = new BookFilter_StringFilter_Matrix
        {
            Title = new StringFilter { Eq = "Clean Code" }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_StringFilter_Equals_Property_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Clean Code" },
            new Book { Id = 2, Title = "Code Complete" },
            new Book { Id = 3, Title = "Clean Code" },
            new Book { Id = 4, Title = "Refactoring" }
        };

        var filter = new BookFilter_StringFilter_Matrix
        {
            Title = new StringFilter { Equals = "Clean Code" }
        };

        AssertParity(books, filter);
    }

    #endregion

    #region CollectionFilterType.Any and Nested Chains

    [Fact]
    public void Parity_CollectionFilterType_Any_NonEmptyCollection_MatchesRuntime()
    {
        var authors = new[]
        {
            new Author
            {
                Id = 1,
                Name = "Author A",
                Books = new List<Book>
                {
                    new Book { Title = "Book 1", Year = 2000 },
                    new Book { Title = "Book 2", Year = 2001 }
                }
            },
            new Author
            {
                Id = 2,
                Name = "Author B",
                Books = new List<Book>
                {
                    new Book { Title = "Book 3", Year = 1999 },
                    new Book { Title = "Book 4", Year = 2005 }
                }
            },
            new Author
            {
                Id = 3,
                Name = "Author C",
                Books = new List<Book>
                {
                    new Book { Title = "Book 5", Year = 2002 }
                }
            }
        };

        var filter = new AuthorFilter_CollectionAny
        {
            Books = new BookNestedFilter
            {
                Year = new Range<int> { Min = 2000 }
            }
        };

        AssertParityAuthors(authors, filter);
    }

    [Fact]
    public void Parity_CollectionFilterType_Any_EmptyCollection_NoMatches()
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
                    new Book { Title = "Book 1", Year = 2000 }
                }
            }
        };

        var filter = new AuthorFilter_CollectionAny
        {
            Books = new BookNestedFilter
            {
                Year = new Range<int> { Min = 2000 }
            }
        };

        AssertParityAuthors(authors, filter);
    }

    [Fact]
    public void Parity_CollectionFilterType_Any_NestedChain_Publisher_Authors_Books()
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
                        Name = "Author A",
                        Books = new List<Book>
                        {
                            new Book { Title = "Clean Code", Year = 2008 },
                            new Book { Title = "Code Complete", Year = 1993 }
                        }
                    },
                    new Author
                    {
                        Id = 2,
                        Name = "Author B",
                        Books = new List<Book>
                        {
                            new Book { Title = "Refactoring", Year = 1999 },
                            new Book { Title = "Patterns", Year = 1994 }
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
                        Name = "Author C",
                        Books = new List<Book>
                        {
                            new Book { Title = "Book X", Year = 2010 }
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
                    Year = new Range<int> { Min = 2000, Max = 2009 }
                }
            }
        };

        var runtime = ExecutePublishers(() => filter.ApplyFilterTo(publishers.AsQueryable()).ToList());
        var generated = ExecutePublishers(() => publishers.AsQueryable().ApplyFilter(filter).ToList());

        Assert.Equal(runtime.Exception?.GetType(), generated.Exception?.GetType());

        if (runtime.Exception != null)
        {
            Assert.Equal(runtime.Exception.Message, generated.Exception.Message);
            return;
        }

        Assert.Equal(runtime.Result.Select(x => x.Id), generated.Result.Select(x => x.Id));
    }

    [Fact]
    public void Parity_CollectionFilterType_Any_MultipleFilters_AllMustMatch()
    {
        var authors = new[]
        {
            new Author
            {
                Id = 1,
                Name = "Author A",
                Books = new List<Book>
                {
                    new Book { Title = "Clean Code", Year = 2008, TotalPage = 400 },
                    new Book { Title = "Code Complete", Year = 1993, TotalPage = 900 }
                }
            },
            new Author
            {
                Id = 2,
                Name = "Author B",
                Books = new List<Book>
                {
                    new Book { Title = "Clean Code", Year = 2008, TotalPage = 200 }
                }
            }
        };

        var filter = new AuthorFilter_CollectionAny
        {
            Books = new BookNestedFilter
            {
                Title = new StringFilter { Contains = "Clean" },
                Year = new Range<int> { Min = 2000 },
                TotalPage = new OperatorFilter<int> { Gt = 300 }
            }
        };

        AssertParityAuthors(authors, filter);
    }

    #endregion

    #region Multi-Target Range CompareTo

    [Fact]
    public void Parity_Range_MultipleTargets_OR_Semantics_MatchesRuntime()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A", TotalPage = 250, ReadCount = 100 },
            new Book { Id = 2, Title = "Book B", TotalPage = 150, ReadCount = 300 },
            new Book { Id = 3, Title = "Book C", TotalPage = 400, ReadCount = 200 },
            new Book { Id = 4, Title = "Book D", TotalPage = 180, ReadCount = 80 }
        };

        var filter = new BookFilter_Range_MultipleProperty
        {
            PageRange = new Range<int> { Min = 200, Max = 300 }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_Range_MinAndMax_BothBoundsEnforced()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A", TotalPage = 150 },
            new Book { Id = 2, Title = "Book B", TotalPage = 250 },
            new Book { Id = 3, Title = "Book C", TotalPage = 350 },
            new Book { Id = 4, Title = "Book D", TotalPage = 200 }
        };

        var filter = new BookFilter_Range
        {
            Year = new Range<int> { Min = 200, Max = 300 }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_Range_OnlyMin_LowerBoundOnly()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A", TotalPage = 150 },
            new Book { Id = 2, Title = "Book B", TotalPage = 250 },
            new Book { Id = 3, Title = "Book C", TotalPage = 350 }
        };

        var filter = new BookFilter_Range
        {
            Year = new Range<int> { Min = 200 }
        };

        AssertParity(books, filter);
    }

    [Fact]
    public void Parity_Range_OnlyMax_UpperBoundOnly()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Book A", TotalPage = 150 },
            new Book { Id = 2, Title = "Book B", TotalPage = 250 },
            new Book { Id = 3, Title = "Book C", TotalPage = 350 }
        };

        var filter = new BookFilter_Range
        {
            Year = new Range<int> { Max = 300 }
        };

        AssertParity(books, filter);
    }

    #endregion

    #region Invalid CompareTo Filterable Type

    [Fact]
    public void Parity_InvalidCompareTo_FilterableTypeMismatch_ThrowsOrIgnores()
    {
        var books = new[]
        {
            new Book { Id = 1, Title = "Test", TotalPage = 100 }
        };

        var filter = new BookFilter_InvalidFilterableType
        {
            Search = "value"
        };

        var runtime = Execute(() => filter.ApplyFilterTo(books.AsQueryable()).ToList());
        var generated = Execute(() => books.AsQueryable().ApplyFilter(filter).ToList());

        Assert.Equal(runtime.Exception?.GetType(), generated.Exception?.GetType());

        if (runtime.Exception != null)
        {
            Assert.Equal(runtime.Exception.Message, generated.Exception.Message);
        }
        else
        {
            Assert.Equal(runtime.Result.Select(x => x.Id), generated.Result.Select(x => x.Id));
        }
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

    private static void AssertParityAuthors<TFilter>(IEnumerable<Author> authors, TFilter filter)
        where TFilter : FilterBase
    {
        var runtime = ExecuteAuthors(() => filter.ApplyFilterTo(authors.AsQueryable()).ToList());
        var generated = ExecuteAuthors(() => authors.AsQueryable().ApplyFilter(filter).ToList());

        Assert.Equal(runtime.Exception?.GetType(), generated.Exception?.GetType());

        if (runtime.Exception != null)
        {
            Assert.Equal(runtime.Exception.Message, generated.Exception.Message);
            return;
        }

        Assert.Equal(runtime.Result.Select(x => x.Id), generated.Result.Select(x => x.Id));
    }

    private static void AssertParityPublishers<TFilter>(IEnumerable<Publisher> publishers, TFilter filter)
        where TFilter : FilterBase
    {
        var runtime = ExecutePublishers(() => filter.ApplyFilterTo(publishers.AsQueryable()).ToList());
        var generated = ExecutePublishers(() => publishers.AsQueryable().ApplyFilter(filter).ToList());

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

    private static ExecutionResultAuthors ExecuteAuthors(Func<List<Author>> run)
    {
        try
        {
            return new ExecutionResultAuthors { Result = run() };
        }
        catch (Exception ex)
        {
            return new ExecutionResultAuthors { Exception = ex };
        }
    }

    private static ExecutionResultPublishers ExecutePublishers(Func<List<Publisher>> run)
    {
        try
        {
            return new ExecutionResultPublishers { Result = run() };
        }
        catch (Exception ex)
        {
            return new ExecutionResultPublishers { Exception = ex };
        }
    }

    private sealed class ExecutionResult
    {
        public List<Book> Result { get; set; } = new List<Book>();
        public Exception Exception { get; set; }
    }

    private sealed class ExecutionResultAuthors
    {
        public List<Author> Result { get; set; } = new List<Author>();
        public Exception Exception { get; set; }
    }

    private sealed class ExecutionResultPublishers
    {
        public List<Publisher> Result { get; set; } = new List<Publisher>();
        public Exception Exception { get; set; }
    }
}
