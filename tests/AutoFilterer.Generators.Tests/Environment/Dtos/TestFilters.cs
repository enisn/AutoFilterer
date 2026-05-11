using AutoFilterer;
using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using AutoFilterer.Types;
using System;
using Book = AutoFilterer.Generators.Tests.Environment.Models.Book;
using Author = AutoFilterer.Generators.Tests.Environment.Models.Author;
using Publisher = AutoFilterer.Generators.Tests.Environment.Models.Publisher;
using Preferences = AutoFilterer.Generators.Tests.Environment.Models.Preferences;
using Level1 = AutoFilterer.Generators.Tests.Environment.Models.Level1;
using Level2 = AutoFilterer.Generators.Tests.Environment.Models.Level2;
using Level3 = AutoFilterer.Generators.Tests.Environment.Models.Level3;
using Level4 = AutoFilterer.Generators.Tests.Environment.Models.Level4;
using Level5 = AutoFilterer.Generators.Tests.Environment.Models.Level5;
using Level6 = AutoFilterer.Generators.Tests.Environment.Models.Level6;
using Level7 = AutoFilterer.Generators.Tests.Environment.Models.Level7;
using Level8 = AutoFilterer.Generators.Tests.Environment.Models.Level8;

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

    [CompareTo(nameof(Book.Views))]
    public Range<int> Views { get; set; }
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

    [CompareTo(nameof(Book.Views))]
    public OperatorFilter<int> Views { get; set; }
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

// CompareTo with multiple properties (Or)
[GenerateApplyFilter(typeof(Book))]
public class BookFilter_MultiplePropertyOr : FilterBase
{
    [CompareTo(nameof(Book.Title), nameof(Book.Author), CombineWith = CombineType.Or)]
    [StringFilterOptions(StringFilterOption.Contains)]
    public string Query { get; set; }
}

// CompareTo with multiple properties (And)
[GenerateApplyFilter(typeof(Book))]
public class BookFilter_MultiplePropertyAnd : FilterBase
{
    [CompareTo(nameof(Book.Title), nameof(Book.Author), CombineWith = CombineType.And)]
    [StringFilterOptions(StringFilterOption.Contains)]
    public string Query { get; set; }
}

// CompareTo with type attribute
[GenerateApplyFilter(typeof(Book))]
public class BookFilter_TypeCompareTo : FilterBase
{
    [CompareTo(typeof(ToLowerContainsComparisonAttribute), nameof(Book.Title))]
    public string Search { get; set; }
}

// Multiple type CompareTo with Or combination
[GenerateApplyFilter(typeof(Book))]
public class BookFilter_MultipleTypeCompareTo : FilterBase
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

// Multiple type CompareTo with And combination
[GenerateApplyFilter(typeof(Book))]
public class BookFilter_MultipleTypeCompareToAnd : FilterBase
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

// Array search without attribute
[GenerateApplyFilter(typeof(Preferences))]
public class PreferencesFilter_ArraySearchWithout : FilterBase
{
    public int[] SecurityLevel { get; set; }
}

// Array search with attribute
[GenerateApplyFilter(typeof(Preferences))]
public class PreferencesFilter_ArraySearchWith : FilterBase
{
    [ArraySearchFilter]
    public int[] SecurityLevel { get; set; }
}

// Array search Guid without attribute
[GenerateApplyFilter(typeof(Preferences))]
public class PreferencesFilter_ArraySearchGuidWithout : FilterBase
{
    public Guid?[] OrganizationUnitId { get; set; }
}

// StringFilter with advanced properties
[GenerateApplyFilter(typeof(Book))]
public class BookFilter_StringFilter_Advanced : FilterBase
{
    [CompareTo(nameof(Book.Title))]
    public StringFilter Title { get; set; }
}

// OperatorFilter with all operators
[GenerateApplyFilter(typeof(Book))]
public class BookFilter_OperatorFilter_Advanced : FilterBase
{
    [CompareTo(nameof(Book.TotalPage))]
    public OperatorFilter<int> TotalPage { get; set; }

    [CompareTo(nameof(Book.Views))]
    public OperatorFilter<int> Views { get; set; }
}

// CompareTo with multiple properties (Range)
[GenerateApplyFilter(typeof(Book))]
public class BookFilter_Range_MultipleProperty : FilterBase
{
    [CompareTo(nameof(Book.TotalPage), nameof(Book.ReadCount))]
    public Range<int> PageRange { get; set; }
}

// StringFilterOptions on string property
[GenerateApplyFilter(typeof(Book))]
public class BookFilter_StringOptionsContains : FilterBase
{
    [CompareTo(nameof(Book.Title))]
    [StringFilterOptions(StringFilterOption.Contains)]
    public string Query { get; set; }
}

public abstract class BookFilter_BaseWithIgnored : FilterBase
{
    [IgnoreFilter]
    [CompareTo(nameof(Book.Title))]
    public virtual string IgnoredQuery { get; set; }
}

[GenerateApplyFilter(typeof(Book))]
public class BookFilter_InheritedIgnore : BookFilter_BaseWithIgnored
{
}

[GenerateApplyFilter(typeof(Book))]
public class BookFilter_DottedPath : FilterBase
{
    [CompareTo("AuthorModel.Name")]
    [StringFilterOptions(StringFilterOption.Contains, StringComparison.InvariantCultureIgnoreCase)]
    public string AuthorName { get; set; }
}

// Filter with invalid CompareTo target for error handling parity test
[GenerateApplyFilter(typeof(Book))]
public class BookFilter_InvalidTarget : FilterBase
{
    [CompareTo("NonExistentProperty")]
    public string Search { get; set; }
}

// Filter for ToLowerEqualsComparison semantics
[GenerateApplyFilter(typeof(Book))]
public class BookFilter_ToLowerEquals : FilterBase
{
    [CompareTo(typeof(ToLowerEqualsComparisonAttribute), nameof(Book.Title))]
    public string Title { get; set; }
}

// Filter for CombineWith at top level across scalar properties
[GenerateApplyFilter(typeof(Book))]
public class BookFilter_ScalarCombineWith : FilterBase
{
    [CompareTo(nameof(Book.Title))]
    [StringFilterOptions(StringFilterOption.Contains, StringComparison.InvariantCultureIgnoreCase)]
    public string Title { get; set; }

    [CompareTo(nameof(Book.Author))]
    [StringFilterOptions(StringFilterOption.Contains, StringComparison.InvariantCultureIgnoreCase)]
    public string Author { get; set; }
}

// Orderable filter for edge cases
[GenerateApplyFilter(typeof(Book))]
public class BookFilter_OrderableEdge : OrderableFilterBase
{
    [CompareTo(nameof(Book.Title))]
    public string Title { get; set; }
}

// Nested non-collection object filter
public class AuthorModelFilter : FilterBase
{
    [CompareTo(nameof(Author.Name))]
    [StringFilterOptions(StringFilterOption.Contains, StringComparison.InvariantCultureIgnoreCase)]
    public string Name { get; set; }
}

[GenerateApplyFilter(typeof(Book))]
public class BookFilter_NestedObject : FilterBase
{
    [CompareTo("AuthorModel")]
    public AuthorModelFilter AuthorFilter { get; set; }
}

// OperatorFilter matrix tests
[GenerateApplyFilter(typeof(Book))]
public class BookFilter_OperatorFilter_Matrix : FilterBase
{
    [CompareTo(nameof(Book.TotalPage))]
    public OperatorFilter<int> TotalPage { get; set; }

    [CompareTo(nameof(Book.Views))]
    public OperatorFilter<int> Views { get; set; }
}

// StringFilter matrix tests
[GenerateApplyFilter(typeof(Book))]
public class BookFilter_StringFilter_Matrix : FilterBase
{
    [CompareTo(nameof(Book.Title))]
    public StringFilter Title { get; set; }
}

// Invalid CompareTo filterable type for error handling tests
[GenerateApplyFilter(typeof(Book))]
public class BookFilter_InvalidFilterableType : FilterBase
{
    [CompareTo(typeof(StringFilter), nameof(Book.TotalPage))]
    public string Search { get; set; }
}

// Implicit nested object mapping tests - property name matches model property without [CompareTo]
public class AuthorFilterImplicit : FilterBase
{
    [CompareTo(nameof(Author.Name))]
    [StringFilterOptions(StringFilterOption.Contains, StringComparison.InvariantCultureIgnoreCase)]
    public string Name { get; set; }

    [CompareTo(nameof(Author.Country))]
    public string Country { get; set; }

    [CompareTo(nameof(Author.Age))]
    public Range<int> Age { get; set; }
}

[GenerateApplyFilter(typeof(Book))]
public class BookFilter_NestedObjectImplicit : FilterBase
{
    // Implicit mapping: property name "AuthorModel" matches Book.AuthorModel without [CompareTo]
    public AuthorFilterImplicit AuthorModel { get; set; }

    public string Title { get; set; }
}

// Implicit nested collection mapping tests - property name matches model property without [CompareTo]
public class BookFilterImplicit : FilterBase
{
    public string Title { get; set; }

    [CompareTo(nameof(Book.Year))]
    public Range<int> Year { get; set; }
}

[GenerateApplyFilter(typeof(Author))]
public class AuthorFilter_CollectionImplicit : FilterBase
{
    public string Name { get; set; }

    // Implicit mapping: property name "Books" matches Author.Books without [CompareTo]
    public BookFilterImplicit Books { get; set; }
}

// Deep nested collection filter with All for empty filter semantics tests
public class AuthorNestedFilter_CollectionAll : FilterBase
{
    [CompareTo(nameof(Author.Name))]
    public StringFilter Name { get; set; }

    [CompareTo(nameof(Author.Books))]
    [CollectionFilter(CollectionFilterType.All)]
    public BookNestedFilter Books { get; set; }
}

[GenerateApplyFilter(typeof(Publisher))]
public class PublisherFilter_NestedCollectionAll : FilterBase
{
    [CompareTo(nameof(Publisher.Name))]
    public string Name { get; set; }

    [CompareTo(nameof(Publisher.Authors))]
    [CollectionFilter(CollectionFilterType.All)]
    public AuthorNestedFilter_CollectionAll Authors { get; set; }
}

public class Level8Filter : FilterBase
{
    [CompareTo(nameof(Level8.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level8.Score))]
    public Range<int> Score { get; set; }
}

public class Level7Filter : FilterBase
{
    [CompareTo(nameof(Level7.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level7.Children))]
    [CollectionFilter(CollectionFilterType.Any)]
    public Level8Filter Children { get; set; }
}

public class Level7Filter_All : FilterBase
{
    [CompareTo(nameof(Level7.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level7.Children))]
    [CollectionFilter(CollectionFilterType.All)]
    public Level8Filter Children { get; set; }
}

public class Level6Filter : FilterBase
{
    [CompareTo(nameof(Level6.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level6.Children))]
    [CollectionFilter(CollectionFilterType.Any)]
    public Level7Filter Children { get; set; }
}

public class Level6Filter_AllAny : FilterBase
{
    [CompareTo(nameof(Level6.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level6.Children))]
    [CollectionFilter(CollectionFilterType.All)]
    public Level7Filter_All Children { get; set; }
}

public class Level5Filter : FilterBase
{
    [CompareTo(nameof(Level5.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level5.Children))]
    [CollectionFilter(CollectionFilterType.Any)]
    public Level6Filter Children { get; set; }
}

public class Level5Filter_AllAnyAny : FilterBase
{
    [CompareTo(nameof(Level5.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level5.Children))]
    [CollectionFilter(CollectionFilterType.All)]
    public Level6Filter_AllAny Children { get; set; }
}

public class Level4Filter : FilterBase
{
    [CompareTo(nameof(Level4.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level4.Children))]
    [CollectionFilter(CollectionFilterType.Any)]
    public Level5Filter Children { get; set; }
}

public class Level4Filter_AllAnyAnyAny : FilterBase
{
    [CompareTo(nameof(Level4.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level4.Children))]
    [CollectionFilter(CollectionFilterType.All)]
    public Level5Filter_AllAnyAny Children { get; set; }
}

public class Level3Filter : FilterBase
{
    [CompareTo(nameof(Level3.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level3.Children))]
    [CollectionFilter(CollectionFilterType.Any)]
    public Level4Filter Children { get; set; }
}

public class Level3Filter_AllAnyAnyAnyAny : FilterBase
{
    [CompareTo(nameof(Level3.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level3.Children))]
    [CollectionFilter(CollectionFilterType.All)]
    public Level4Filter_AllAnyAnyAny Children { get; set; }
}

public class Level2Filter : FilterBase
{
    [CompareTo(nameof(Level2.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level2.Children))]
    [CollectionFilter(CollectionFilterType.Any)]
    public Level3Filter Children { get; set; }
}

public class Level2Filter_AllAnyAnyAnyAnyAny : FilterBase
{
    [CompareTo(nameof(Level2.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level2.Children))]
    [CollectionFilter(CollectionFilterType.All)]
    public Level3Filter_AllAnyAnyAnyAny Children { get; set; }
}

[GenerateApplyFilter(typeof(Level1))]
public class Level1Filter_AnyAllMixed : FilterBase
{
    [CompareTo(nameof(Level1.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level1.Children))]
    [CollectionFilter(CollectionFilterType.Any)]
    public Level2Filter Children { get; set; }
}

[GenerateApplyFilter(typeof(Level1))]
public class Level1Filter_AllAnyAnyAnyAnyAnyAny : FilterBase
{
    [CompareTo(nameof(Level1.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level1.Children))]
    [CollectionFilter(CollectionFilterType.All)]
    public Level2Filter_AllAnyAnyAnyAnyAny Children { get; set; }
}

// Alternating Any/All pattern: Any-Any-All-Any-Any-All
public class Level6Filter_AltPattern : FilterBase
{
    [CompareTo(nameof(Level6.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level6.Children))]
    [CollectionFilter(CollectionFilterType.All)]
    public Level7Filter_All Children { get; set; }
}

public class Level5Filter_AltPattern : FilterBase
{
    [CompareTo(nameof(Level5.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level5.Children))]
    [CollectionFilter(CollectionFilterType.Any)]
    public Level6Filter_AltPattern Children { get; set; }
}

public class Level4Filter_AltPattern : FilterBase
{
    [CompareTo(nameof(Level4.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level4.Children))]
    [CollectionFilter(CollectionFilterType.Any)]
    public Level5Filter_AltPattern Children { get; set; }
}

public class Level3Filter_AltPattern : FilterBase
{
    [CompareTo(nameof(Level3.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level3.Children))]
    [CollectionFilter(CollectionFilterType.All)]
    public Level4Filter_AltPattern Children { get; set; }
}

public class Level2Filter_AltPattern : FilterBase
{
    [CompareTo(nameof(Level2.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level2.Children))]
    [CollectionFilter(CollectionFilterType.Any)]
    public Level3Filter_AltPattern Children { get; set; }
}

[GenerateApplyFilter(typeof(Level1))]
public class Level1Filter_AltPattern : FilterBase
{
    [CompareTo(nameof(Level1.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level1.Children))]
    [CollectionFilter(CollectionFilterType.Any)]
    public Level2Filter_AltPattern Children { get; set; }
}

// All-All pattern (all quantifiers are All)
public class Level6Filter_AllAll : FilterBase
{
    [CompareTo(nameof(Level6.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level6.Children))]
    [CollectionFilter(CollectionFilterType.All)]
    public Level7Filter_All Children { get; set; }
}

public class Level5Filter_AllAll : FilterBase
{
    [CompareTo(nameof(Level5.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level5.Children))]
    [CollectionFilter(CollectionFilterType.All)]
    public Level6Filter_AllAll Children { get; set; }
}

public class Level4Filter_AllAll : FilterBase
{
    [CompareTo(nameof(Level4.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level4.Children))]
    [CollectionFilter(CollectionFilterType.All)]
    public Level5Filter_AllAll Children { get; set; }
}

public class Level3Filter_AllAll : FilterBase
{
    [CompareTo(nameof(Level3.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level3.Children))]
    [CollectionFilter(CollectionFilterType.All)]
    public Level4Filter_AllAll Children { get; set; }
}

public class Level2Filter_AllAll : FilterBase
{
    [CompareTo(nameof(Level2.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level2.Children))]
    [CollectionFilter(CollectionFilterType.All)]
    public Level3Filter_AllAll Children { get; set; }
}

[GenerateApplyFilter(typeof(Level1))]
public class Level1Filter_AllAll : FilterBase
{
    [CompareTo(nameof(Level1.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level1.Children))]
    [CollectionFilter(CollectionFilterType.All)]
    public Level2Filter_AllAll Children { get; set; }
}

// Prefix-Any-Suffix-All pattern: Any-Any-Any-Any-All-All-All-All (depth 8)
// This tests the boundary where quantifiers switch from Any to All at a specific depth
public class Level7Filter_PrefixAnySuffixAll : FilterBase
{
    [CompareTo(nameof(Level7.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level7.Children))]
    [CollectionFilter(CollectionFilterType.All)]
    public Level8Filter Children { get; set; }
}

public class Level6Filter_PrefixAnySuffixAll : FilterBase
{
    [CompareTo(nameof(Level6.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level6.Children))]
    [CollectionFilter(CollectionFilterType.All)]
    public Level7Filter_PrefixAnySuffixAll Children { get; set; }
}

public class Level5Filter_PrefixAnySuffixAll : FilterBase
{
    [CompareTo(nameof(Level5.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level5.Children))]
    [CollectionFilter(CollectionFilterType.All)]
    public Level6Filter_PrefixAnySuffixAll Children { get; set; }
}

public class Level4Filter_PrefixAnySuffixAll : FilterBase
{
    [CompareTo(nameof(Level4.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level4.Children))]
    [CollectionFilter(CollectionFilterType.All)]
    public Level5Filter_PrefixAnySuffixAll Children { get; set; }
}

public class Level3Filter_PrefixAnySuffixAll : FilterBase
{
    [CompareTo(nameof(Level3.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level3.Children))]
    [CollectionFilter(CollectionFilterType.Any)]
    public Level4Filter_PrefixAnySuffixAll Children { get; set; }
}

public class Level2Filter_PrefixAnySuffixAll : FilterBase
{
    [CompareTo(nameof(Level2.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level2.Children))]
    [CollectionFilter(CollectionFilterType.Any)]
    public Level3Filter_PrefixAnySuffixAll Children { get; set; }
}

[GenerateApplyFilter(typeof(Level1))]
public class Level1Filter_PrefixAnySuffixAll : FilterBase
{
    [CompareTo(nameof(Level1.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level1.Children))]
    [CollectionFilter(CollectionFilterType.Any)]
    public Level2Filter_PrefixAnySuffixAll Children { get; set; }
}

// Sparse flips pattern: Any-Any-All-Any-All-All-Any-All (depth 8)
// This tests irregular quantifier flips across depth
public class Level7Filter_SparseFlips : FilterBase
{
    [CompareTo(nameof(Level7.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level7.Children))]
    [CollectionFilter(CollectionFilterType.Any)]
    public Level8Filter Children { get; set; }
}

public class Level6Filter_SparseFlips : FilterBase
{
    [CompareTo(nameof(Level6.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level6.Children))]
    [CollectionFilter(CollectionFilterType.All)]
    public Level7Filter_SparseFlips Children { get; set; }
}

public class Level5Filter_SparseFlips : FilterBase
{
    [CompareTo(nameof(Level5.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level5.Children))]
    [CollectionFilter(CollectionFilterType.Any)]
    public Level6Filter_SparseFlips Children { get; set; }
}

public class Level4Filter_SparseFlips : FilterBase
{
    [CompareTo(nameof(Level4.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level4.Children))]
    [CollectionFilter(CollectionFilterType.All)]
    public Level5Filter_SparseFlips Children { get; set; }
}

public class Level3Filter_SparseFlips : FilterBase
{
    [CompareTo(nameof(Level3.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level3.Children))]
    [CollectionFilter(CollectionFilterType.Any)]
    public Level4Filter_SparseFlips Children { get; set; }
}

public class Level2Filter_SparseFlips : FilterBase
{
    [CompareTo(nameof(Level2.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level2.Children))]
    [CollectionFilter(CollectionFilterType.Any)]
    public Level3Filter_SparseFlips Children { get; set; }
}

[GenerateApplyFilter(typeof(Level1))]
public class Level1Filter_SparseFlips : FilterBase
{
    [CompareTo(nameof(Level1.Value))]
    public StringFilter Value { get; set; }

    [CompareTo(nameof(Level1.Children))]
    [CollectionFilter(CollectionFilterType.Any)]
    public Level2Filter_SparseFlips Children { get; set; }
}

