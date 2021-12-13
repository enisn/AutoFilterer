using AutoFilterer.Tests.Core;
using AutoFilterer.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using Castle.Core.Internal;
using Xunit;

namespace AutoFilterer.Generators.Tests;

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

public class FilterGeneratorTests
{
    [Fact]
    public void ShouldBookFilterBeCreated()
    {
        // If there is no compile error. Everything is OK 👍
        Assert.True(typeof(BookFilter) != null);
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
        var type = typeof(BookFilter);

        Assert.True(type.GetProperty(nameof(Book.Title)).PropertyType == typeof(string));
    }

    [Theory]
    [AutoMoqData]
    public void Test(List<Book> books)
    {
        var filter = new BookFilter();
        filter.Page = 1;
        filter.PerPage = 2;
        filter.Year = new Types.Range<int>(min: 1990, max: 2021);

        books.AsQueryable().ApplyFilter(filter);
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

    [Fact]
    public void ShouldCreateEachTypeCorrectFromMapping()
    {
        Assert.NotNull(typeof(MappingTest.AllTypesTestTypeFilter));

        var filter = new MappingTest.AllTypesTestTypeFilter();
    }

    [GenerateAutoFilter("MappingTest")]
    public class StringAttributeTestType
    {
        public string Title { get; set; }
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
}

