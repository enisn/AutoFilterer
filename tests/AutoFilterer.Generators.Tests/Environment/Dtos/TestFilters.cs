using AutoFilterer;
using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using AutoFilterer.Types;
using Book = AutoFilterer.Generators.Tests.Environment.Models.Book;
using Author = AutoFilterer.Generators.Tests.Environment.Models.Author;
using Publisher = AutoFilterer.Generators.Tests.Environment.Models.Publisher;

namespace AutoFilterer.Generators.Tests.Environment.Dtos;

// Basic scalar filter
[GenerateApplyFilter(typeof(Book))]
public class BookFilter_Basic : FilterBase
{
    public string Title { get; set; }
}

// StringFilter tests
[GenerateApplyFilter(typeof(Book))]
public class BookFilter_StringFilter : FilterBase
{
    [CompareTo(nameof(Book.Title))]
    public StringFilter Title { get; set; }
}

// OperatorFilter tests
[GenerateApplyFilter(typeof(Book))]
public class BookFilter_OperatorFilter : FilterBase
{
    [CompareTo(nameof(Book.TotalPage))]
    public OperatorFilter<int> TotalPage { get; set; }
    
    [CompareTo(nameof(Book.Year))]
    public OperatorFilter<int> Year { get; set; }
}

// Range tests
[GenerateApplyFilter(typeof(Book))]
public class BookFilter_Range : FilterBase
{
    [CompareTo(nameof(Book.Year))]
    public Range<int> Year { get; set; }
    
    [CompareTo(nameof(Book.TotalPage))]
    public Range<int> TotalPage { get; set; }
}

// Orderable tests
[GenerateApplyFilter(typeof(Book))]
public class BookFilter_Orderable : OrderableFilterBase
{
    public string Title { get; set; }
}

// Pagination tests
[GenerateApplyFilter(typeof(Book))]
public class BookFilter_Pagination : PaginationFilterBase
{
    public string Title { get; set; }
}

// Nested collection filter - Book nested
public class BookNestedFilter : FilterBase
{
    [CompareTo(nameof(Book.Title))]
    public StringFilter Title { get; set; }
    
    [CompareTo(nameof(Book.Year))]
    public Range<int> Year { get; set; }
    
    [CompareTo(nameof(Book.TotalPage))]
    public OperatorFilter<int> TotalPage { get; set; }
}

// Collection filter tests
[GenerateApplyFilter(typeof(Author))]
public class AuthorFilter_CollectionAny : FilterBase
{
    [CompareTo(nameof(Author.Name))]
    public StringFilter Name { get; set; }
    
    [CompareTo(nameof(Author.Books))]
    [CollectionFilter(CollectionFilterType.Any)]
    public BookNestedFilter Books { get; set; }
}

[GenerateApplyFilter(typeof(Author))]
public class AuthorFilter_CollectionAll : FilterBase
{
    [CompareTo(nameof(Author.Name))]
    public string Name { get; set; }
    
    [CompareTo(nameof(Author.Books))]
    [CollectionFilter(CollectionFilterType.All)]
    public BookNestedFilter Books { get; set; }
}

// Complex nested filter
public class AuthorNestedFilter : FilterBase
{
    [CompareTo(nameof(Author.Name))]
    public StringFilter Name { get; set; }
    
    [CompareTo(nameof(Author.Books))]
    [CollectionFilter(CollectionFilterType.Any)]
    public BookNestedFilter Books { get; set; }
}

[GenerateApplyFilter(typeof(Publisher))]
public class PublisherFilter_NestedCollection : FilterBase
{
    [CompareTo(nameof(Publisher.Name))]
    public string Name { get; set; }
    
    [CompareTo(nameof(Publisher.Authors))]
    [CollectionFilter(CollectionFilterType.Any)]
    public AuthorNestedFilter Authors { get; set; }
}

// Multiple property mapping
[GenerateApplyFilter(typeof(Book))]
public class BookFilter_MultiplePropertyOr : FilterBase
{
    [CompareTo(nameof(Book.Title))]
    public string SearchText { get; set; }
}

// Combined filter with all features
[GenerateApplyFilter(typeof(Author))]
public class AuthorFilter_Complete : PaginationFilterBase
{
    [CompareTo(nameof(Author.Name))]
    public StringFilter Name { get; set; }
    
    [CompareTo(nameof(Author.Country))]
    public string Country { get; set; }
    
    [CompareTo(nameof(Author.Age))]
    public OperatorFilter<int> Age { get; set; }
    
    [CompareTo(nameof(Author.Books))]
    [CollectionFilter(CollectionFilterType.Any)]
    public BookNestedFilter Books { get; set; }
}
