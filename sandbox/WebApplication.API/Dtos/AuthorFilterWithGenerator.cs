using AutoFilterer.Attributes;
using AutoFilterer.Types;
using AutoFilterer;
using AutoFilterer.Enums;
using WebApplication.API.Models;

namespace WebApplication.API.Dtos;

[GenerateApplyFilter(typeof(Author))]
public class AuthorFilterWithGenerator : PaginationFilterBase
{
    [CompareTo(nameof(Author.Name))]
    public StringFilter Name { get; set; }

    [CompareTo(nameof(Author.Country))]
    public string Country { get; set; }

    [CompareTo(nameof(Author.Books))]
    [CollectionFilter(CollectionFilterType.Any)]
    public BookNestedFilter Books { get; set; }
}

public class BookNestedFilter : FilterBase
{
    [CompareTo(nameof(Book.Title))]
    public StringFilter Title { get; set; }

    [CompareTo(nameof(Book.Year))]
    public Range<int> Year { get; set; }

    [CompareTo(nameof(Book.TotalPage))]
    public OperatorFilter<int> TotalPage { get; set; }
}
