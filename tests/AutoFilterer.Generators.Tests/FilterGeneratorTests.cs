using AutoFilterer.Tests.Core;
using AutoFilterer.Extensions;
using AutoFilterer.Generators.Tests.Environment.Dtos;
using AutoFilterer.Generators.Tests.Environment.Models;
using AutoFilterer.Types;
using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using Castle.Core.Internal;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoFilterer.Generators.Tests.FilterGenerator_TestClasses
{
    [GenerateAutoFilter]
    public class Book
    {
        public string Title { get; set; }
        public int? Year { get; set; }
        public int TotalPage { get; set; }
        public DateTime PublishTime { get; set; }
    }

    [GenerateAutoFilter("MyCustomNamespace")]
    public class BookInCustomNamespace
    {
        public string Title { get; set; }
        public int? Year { get; set; }
        public int TotalPage { get; set; }
        public DateTime PublishTime { get; set; }
    }

    [GenerateAutoFilter("MappingTest")]
    public class AllTypesTestType
    {
        public sbyte _Sbyte { get; set; }
        public sbyte? _SbyteN { get; set; }
        public byte _Byte { get; set; }
        public byte? _ByteN { get; set; }
        public short _Short { get; set; }
        public short? _ShortN { get; set; }
        public ushort _Ushort { get; set; }
        public ushort? _UshortN { get; set; }
        public int _Int { get; set; }
        public int? _IntN { get; set; }
        public uint _UInt { get; set; }
        public uint? _UIntN { get; set; }
        public long _Long { get; set; }
        public long? _LongN { get; set; }
        public ulong _ULong { get; set; }
        public ulong? _ULongN { get; set; }
        public double _Double { get; set; }
        public double? _DoubleN { get; set; }
        public float _Float { get; set; }
        public float? _FloatN { get; set; }
        public decimal _Decimal { get; set; }
        public decimal? _DecimalN { get; set; }
        public DateTime _DateTime { get; set; }
        public DateTime? _DateTimeN { get; set; }
        public TimeSpan _TimeSpan { get; set; }
        public TimeSpan? _TimeSpanN { get; set; }
    }

    [GenerateAutoFilter("MappingTest")]
    public class StringAttributeTestType
    {
        public string Title { get; set; }
    }

    // Test class with expanded type mappings (bool, Guid, DateTimeOffset, enum)
    [GenerateAutoFilter("ExpandedMappingTest")]
    public class ExpandedTypeTestType
    {
        public bool IsActive { get; set; }
        public bool? IsActiveNullable { get; set; }
        public System.Guid Id { get; set; }
        public System.Guid? IdNullable { get; set; }
        public System.DateTimeOffset CreatedAt { get; set; }
        public System.DateTimeOffset? CreatedAtNullable { get; set; }
        public TestEnum Status { get; set; }
        public TestEnum? StatusNullable { get; set; }
    }

    public enum TestEnum
    {
        Active,
        Inactive,
        Pending
    }

    // Test class with collection and navigation properties (should be skipped)
    [GenerateAutoFilter("NavigationTest")]
    public class NavigationTestType
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public System.Collections.Generic.List<string> Items { get; set; }
        public ComplexType Related { get; set; }
    }

    public class ComplexType
    {
        public string Property { get; set; }
    }

    // Test class with UseStringFilter option
    [GenerateAutoFilter(UseStringFilter = true)]
    public class UseStringFilterTestType
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    // Test class with UseRangeForNumbers = false
    [GenerateAutoFilter(UseRangeForNumbers = false)]
    public class NoRangeForNumbersTestType
    {
        public int Count { get; set; }
        public int? CountNullable { get; set; }
        public decimal Price { get; set; }
    }

    // Test class with UseRangeForDates = false
    [GenerateAutoFilter(UseRangeForDates = false)]
    public class NoRangeForDatesTestType
    {
        public System.DateTime CreatedDate { get; set; }
        public System.DateTime? UpdatedDate { get; set; }
        public System.DateTimeOffset Timestamp { get; set; }
    }

    // Test class with GenerateForEnumProperties = false
    [GenerateAutoFilter(GenerateForEnumProperties = false)]
    public class NoEnumPropertiesTestType
    {
        public string Name { get; set; }
        public TestEnum Status { get; set; }
        public TestEnum? Category { get; set; }
    }

    // Test class with custom base class
    [GenerateAutoFilter(BaseClass = "FilterBase")]
    public class CustomBaseClassTestType
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }

    // Test class combining multiple options
    [GenerateAutoFilter(UseStringFilter = true, UseRangeForNumbers = false, UseRangeForDates = false)]
    public class CombinedOptionsTestType
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public System.DateTime Date { get; set; }
        public TestEnum Status { get; set; }
    }
}

namespace AutoFilterer.Generators.Tests
{
    public class FilterGeneratorTests
    {
        [Fact]
        public void ShouldBookFilterBeCreated()
        {
            // If there is no compile error. Everything is OK 👍
            Assert.True(typeof(FilterGenerator_TestClasses.BookFilter) != null);
        }

        [Fact]
        public void ShouldBookFilterInCustomNamespaceBeCreated()
        {
            // If there is no compile error. Everything is OK 👍
            Assert.True(typeof(MyCustomNamespace.BookInCustomNamespaceFilter) != null);
        }

        [Fact]
        public void ShouldTitleBeString()
        {
            var type = typeof(FilterGenerator_TestClasses.BookFilter);

            Assert.True(type.GetProperty(nameof(FilterGenerator_TestClasses.Book.Title)).PropertyType == typeof(string));
        }

        [Theory]
        [AutoMoqData]
        public void Test(List<FilterGenerator_TestClasses.Book> books)
        {
            var filter = new FilterGenerator_TestClasses.BookFilter();
            filter.Page = 1;
            filter.PerPage = 2;
            filter.Year = new Types.Range<int>(min: 1990, max: 2021);

            books.AsQueryable().ApplyFilter(filter);
        }

        [Fact]
        public void ShouldCreateEachTypeCorrectFromMapping()
        {
            Assert.NotNull(typeof(MappingTest.AllTypesTestTypeFilter));

            var filter = new MappingTest.AllTypesTestTypeFilter();
            Assert.NotNull(filter);
        }

        [Fact]
        public void ShouldHaveToLowerContainsComparisonAttribute()
        {
            var attribute =
                typeof(MappingTest.StringAttributeTestTypeFilter)
                .GetProperty(nameof(MappingTest.StringAttributeTestTypeFilter.Title))
                .GetAttribute<ToLowerContainsComparisonAttribute>();

            Assert.NotNull(attribute);
        }

        [Fact]
        public void ShouldCreateBookFilter_MultiplePropertyOr()
        {
            Assert.NotNull(typeof(BookFilter_MultiplePropertyOr));
        }

        [Fact]
        public void ShouldCreateBookFilter_MultiplePropertyAnd()
        {
            Assert.NotNull(typeof(BookFilter_MultiplePropertyAnd));
        }

        [Fact]
        public void ShouldCreateBookFilter_TypeCompareTo()
        {
            Assert.NotNull(typeof(BookFilter_TypeCompareTo));
        }

        [Fact]
        public void ShouldCreateBookFilter_MultipleTypeCompareTo()
        {
            Assert.NotNull(typeof(BookFilter_MultipleTypeCompareTo));
        }

        [Fact]
        public void ShouldCreateBookFilter_MultipleTypeCompareToAnd()
        {
            Assert.NotNull(typeof(BookFilter_MultipleTypeCompareToAnd));
        }

        [Fact]
        public void ShouldCreatePreferencesFilter_ArraySearchWithout()
        {
            Assert.NotNull(typeof(PreferencesFilter_ArraySearchWithout));
        }

        [Fact]
        public void ShouldCreatePreferencesFilter_ArraySearchWith()
        {
            Assert.NotNull(typeof(PreferencesFilter_ArraySearchWith));
        }

        [Fact]
        public void ShouldCreatePreferencesFilter_ArraySearchGuidWithout()
        {
            Assert.NotNull(typeof(PreferencesFilter_ArraySearchGuidWithout));
        }

        [Fact]
        public void ShouldCreateBookFilter_StringFilter_Advanced()
        {
            Assert.NotNull(typeof(BookFilter_StringFilter_Advanced));
        }

        [Fact]
        public void ShouldCreateBookFilter_OperatorFilter_Advanced()
        {
            Assert.NotNull(typeof(BookFilter_OperatorFilter_Advanced));
        }

        [Fact]
        public void ShouldCreateBookFilter_Range_MultipleProperty()
        {
            Assert.NotNull(typeof(BookFilter_Range_MultipleProperty));
        }

        // Expanded type mapping tests
        [Fact]
        public void ShouldCreateExpandedTypeTestTypeFilter()
        {
            Assert.NotNull(typeof(ExpandedMappingTest.ExpandedTypeTestTypeFilter));
        }

        [Fact]
        public void ShouldHaveBoolPropertyInExpandedTypeFilter()
        {
            var filter = typeof(ExpandedMappingTest.ExpandedTypeTestTypeFilter);
            var isActiveProperty = filter.GetProperty("IsActive");
            Assert.NotNull(isActiveProperty);
            Assert.Equal(typeof(bool), isActiveProperty.PropertyType);
        }

        [Fact]
        public void ShouldHaveGuidPropertyInExpandedTypeFilter()
        {
            var filter = typeof(ExpandedMappingTest.ExpandedTypeTestTypeFilter);
            var idProperty = filter.GetProperty("Id");
            Assert.NotNull(idProperty);
            Assert.Equal(typeof(System.Guid), idProperty.PropertyType);
        }

        [Fact]
        public void ShouldHaveDateTimeOffsetPropertyInExpandedTypeFilter()
        {
            var filter = typeof(ExpandedMappingTest.ExpandedTypeTestTypeFilter);
            var createdAtProperty = filter.GetProperty("CreatedAt");
            Assert.NotNull(createdAtProperty);
            // DateTime should use Range<DateTimeOffset>
            Assert.Equal("Range`1", createdAtProperty.PropertyType.Name);
        }

        [Fact]
        public void ShouldHaveEnumPropertyInExpandedTypeFilter()
        {
            var filter = typeof(ExpandedMappingTest.ExpandedTypeTestTypeFilter);
            var statusProperty = filter.GetProperty("Status");
            Assert.NotNull(statusProperty);
            // Enums should be kept as-is
            Assert.Equal(typeof(FilterGenerator_TestClasses.TestEnum), statusProperty.PropertyType);
        }

        // Navigation property tests
        [Fact]
        public void ShouldCreateNavigationTestTypeFilter()
        {
            Assert.NotNull(typeof(NavigationTest.NavigationTestTypeFilter));
        }

        [Fact]
        public void ShouldNotHaveCollectionPropertyInNavigationTestTypeFilter()
        {
            var filter = typeof(NavigationTest.NavigationTestTypeFilter);
            var itemsProperty = filter.GetProperty("Items");
            Assert.Null(itemsProperty);
        }

        [Fact]
        public void ShouldNotHaveNavigationPropertyInNavigationTestTypeFilter()
        {
            var filter = typeof(NavigationTest.NavigationTestTypeFilter);
            var relatedProperty = filter.GetProperty("Related");
            Assert.Null(relatedProperty);
        }

        // UseStringFilter option tests
        [Fact]
        public void ShouldCreateUseStringFilterTestTypeFilter()
        {
            Assert.NotNull(typeof(FilterGenerator_TestClasses.UseStringFilterTestTypeFilter));
        }

        [Fact]
        public void ShouldHaveStringFilterPropertyInUseStringFilterTestTypeFilter()
        {
            var filter = typeof(FilterGenerator_TestClasses.UseStringFilterTestTypeFilter);
            var nameProperty = filter.GetProperty("Name");
            Assert.NotNull(nameProperty);
            Assert.Equal(typeof(StringFilter), nameProperty.PropertyType);
        }

        // UseRangeForNumbers option tests
        [Fact]
        public void ShouldCreateNoRangeForNumbersTestTypeFilter()
        {
            Assert.NotNull(typeof(FilterGenerator_TestClasses.NoRangeForNumbersTestTypeFilter));
        }

        [Fact]
        public void ShouldHaveIntInsteadOfRangeWhenUseRangeForNumbersIsFalse()
        {
            var filter = typeof(FilterGenerator_TestClasses.NoRangeForNumbersTestTypeFilter);
            var countProperty = filter.GetProperty("Count");
            Assert.NotNull(countProperty);
            Assert.Equal(typeof(int), countProperty.PropertyType);
        }

        // UseRangeForDates option tests
        [Fact]
        public void ShouldCreateNoRangeForDatesTestTypeFilter()
        {
            Assert.NotNull(typeof(FilterGenerator_TestClasses.NoRangeForDatesTestTypeFilter));
        }

        [Fact]
        public void ShouldHaveDateTimeInsteadOfRangeWhenUseRangeForDatesIsFalse()
        {
            var filter = typeof(FilterGenerator_TestClasses.NoRangeForDatesTestTypeFilter);
            var createdDateProperty = filter.GetProperty("CreatedDate");
            Assert.NotNull(createdDateProperty);
            Assert.Equal(typeof(System.DateTime), createdDateProperty.PropertyType);
        }

        // GenerateForEnumProperties option tests
        [Fact]
        public void ShouldCreateNoEnumPropertiesTestTypeFilter()
        {
            Assert.NotNull(typeof(FilterGenerator_TestClasses.NoEnumPropertiesTestTypeFilter));
        }

        [Fact]
        public void ShouldNotHaveEnumPropertyWhenGenerateForEnumPropertiesIsFalse()
        {
            var filter = typeof(FilterGenerator_TestClasses.NoEnumPropertiesTestTypeFilter);
            var statusProperty = filter.GetProperty("Status");
            Assert.Null(statusProperty);
        }

        // Custom base class tests
        [Fact]
        public void ShouldCreateCustomBaseClassTestTypeFilter()
        {
            Assert.NotNull(typeof(FilterGenerator_TestClasses.CustomBaseClassTestTypeFilter));
        }

        [Fact]
        public void ShouldHaveCustomBaseClass()
        {
            var filter = typeof(FilterGenerator_TestClasses.CustomBaseClassTestTypeFilter);
            Assert.Equal(typeof(FilterBase), filter.BaseType);
        }

        // Combined options tests
        [Fact]
        public void ShouldCreateCombinedOptionsTestTypeFilter()
        {
            Assert.NotNull(typeof(FilterGenerator_TestClasses.CombinedOptionsTestTypeFilter));
        }

        [Fact]
        public void ShouldRespectAllCombinedOptions()
        {
            var filter = typeof(FilterGenerator_TestClasses.CombinedOptionsTestTypeFilter);

            // String should be StringFilter
            var nameProperty = filter.GetProperty("Name");
            Assert.Equal(typeof(StringFilter), nameProperty.PropertyType);

            // int should be int (not Range)
            var countProperty = filter.GetProperty("Count");
            Assert.Equal(typeof(int), countProperty.PropertyType);

            // DateTime should be DateTime (not Range)
            var dateProperty = filter.GetProperty("Date");
            Assert.Equal(typeof(System.DateTime), dateProperty.PropertyType);

            // Enum should be present
            var statusProperty = filter.GetProperty("Status");
            Assert.NotNull(statusProperty);
        }

        // Backward compatibility tests
        [Fact]
        public void DefaultUseRangeForNumbers_ShouldUseRange()
        {
            var filter = typeof(FilterGenerator_TestClasses.BookFilter);
            var yearProperty = filter.GetProperty(nameof(FilterGenerator_TestClasses.Book.Year));
            Assert.NotNull(yearProperty);
            Assert.Equal("Range`1", yearProperty.PropertyType.Name);
        }

        [Fact]
        public void DefaultUseStringFilter_ShouldUseStringType()
        {
            var filter = typeof(FilterGenerator_TestClasses.BookFilter);
            var titleProperty = filter.GetProperty(nameof(FilterGenerator_TestClasses.Book.Title));
            Assert.NotNull(titleProperty);
            Assert.Equal(typeof(string), titleProperty.PropertyType);
        }
    }
}
