using AutoFilterer.Enums;
using AutoFilterer.Extensions;
using AutoFilterer.Tests.Environment.Dtos;
using AutoFilterer.Tests.Environment.Models;
using AutoFilterer.Types;
using AutoFixture;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Castle.Core.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoFilterer.Benchmark;

[SimpleJob(RuntimeMoniker.HostProcess)]
[DisassemblyDiagnoser]
[MemoryDiagnoser]
[ThreadingDiagnoser]
public class AutoFiltererStartup
{
    protected IQueryable<Book> books;
    protected BookFilter_ReadCount readCountFilter;
    protected BookFilter_StringFilter_Title stringFilter;
    protected BookFilter_RangeOfPage rangeFilter;
    protected BookFilterBase complexFilter;

    [GlobalSetup]
    public void Setup()
    {
        var fixture = new Fixture();

        books = fixture.Create<List<Book>>().AsQueryable();
        readCountFilter = fixture.Create<BookFilter_ReadCount>();
        stringFilter = fixture.Create<BookFilter_StringFilter_Title>();
        rangeFilter = fixture.Create<BookFilter_RangeOfPage>();
        complexFilter = fixture.Create<BookFilterBase>();
    }

    [Benchmark]
    public void EqualsSingleProperly_Lambda()
    {
        var query = books.Where(x => x.ReadCount == readCountFilter.ReadCount);
    }

    [Benchmark]
    public void EqualsSingleProperly_AutoFilterer()
    {
        var query = books.ApplyFilter(readCountFilter);
    }

    [Benchmark]
    public void StringFilter_Lambda()
    {
        var query = books.AsQueryable();

        if (stringFilter.Title.CombineWith == CombineType.And)
        {
            if (!stringFilter.Title.Contains.IsNullOrEmpty())
            {
                if (stringFilter.Title.Compare != null)
                {
                    query = query.Where(x => x.Title.Contains(stringFilter.Title.Contains));
                }
                else
                {
                    query = query.Where(x => x.Title.Contains(stringFilter.Title.Contains, stringFilter.Title.Compare.Value));
                }
            }

            if (!stringFilter.Title.EndsWith.IsNullOrEmpty())
            {
                if (stringFilter.Title.Compare != null)
                {
                    query = query.Where(x => x.Title.EndsWith(stringFilter.Title.EndsWith));
                }
                else
                {
                    query = query.Where(x => x.Title.EndsWith(stringFilter.Title.EndsWith, stringFilter.Title.Compare.Value));
                }
            }

            if (!stringFilter.Title.Eq.IsNullOrEmpty())
            {
                query = query.Where(x => x.Title == stringFilter.Title.Eq);

            }

            if (!stringFilter.Title.Equals.IsNullOrEmpty())
            {
                if (stringFilter.Title.Compare != null)
                {
                    query = query.Where(x => x.Title.Equals(stringFilter.Title.Eq));
                }
                else
                {
                    query = query.Where(x => x.Title.Equals(stringFilter.Title.Equals, stringFilter.Title.Compare.Value));
                }
            }

            if (!stringFilter.Title.Not.IsNullOrEmpty())
            {
                query = query.Where(x => x.Title != stringFilter.Title.Not);
            }

            if (!stringFilter.Title.StartsWith.IsNullOrEmpty())
            {
                if (stringFilter.Title.Compare != null)
                {
                    query = query.Where(x => x.Title.StartsWith(stringFilter.Title.StartsWith));
                }
                else
                {
                    query = query.Where(x => x.Title.StartsWith(stringFilter.Title.StartsWith, stringFilter.Title.Compare.Value));
                }
            }
        }
        else
        {
            if (stringFilter.Title.Compare != null)
            {
                query = query.Where(x => x.Title.Contains(stringFilter.Title.Contains) || stringFilter.Title.Contains.IsNullOrEmpty());
            }
            else
            {
                query = query.Where(x => x.Title.Contains(stringFilter.Title.Contains, stringFilter.Title.Compare.Value) || stringFilter.Title.Contains.IsNullOrEmpty());
            }

            if (stringFilter.Title.Compare != null)
            {
                query = query.Where(x => x.Title.EndsWith(stringFilter.Title.EndsWith) || stringFilter.Title.EndsWith.IsNullOrEmpty());
            }
            else
            {
                query = query.Where(x => x.Title.EndsWith(stringFilter.Title.EndsWith, stringFilter.Title.Compare.Value) || stringFilter.Title.EndsWith.IsNullOrEmpty());
            }

            if (!stringFilter.Title.Eq.IsNullOrEmpty())
            {
                query = query.Where(x => x.Title == stringFilter.Title.Eq);

            }

            if (stringFilter.Title.Compare != null)
            {
                query = query.Where(x => x.Title.Equals(stringFilter.Title.Eq) || stringFilter.Title.Equals.IsNullOrEmpty());
            }
            else
            {
                query = query.Where(x => x.Title.Equals(stringFilter.Title.Equals, stringFilter.Title.Compare.Value) || stringFilter.Title.Equals.IsNullOrEmpty());
            }

            query = query.Where(x => x.Title != stringFilter.Title.Not || stringFilter.Title.Not.IsNullOrEmpty());

            if (stringFilter.Title.Compare != null)
            {
                query = query.Where(x => x.Title.StartsWith(stringFilter.Title.StartsWith) || stringFilter.Title.StartsWith.IsNullOrEmpty());
            }
            else
            {
                query = query.Where(x => x.Title.StartsWith(stringFilter.Title.StartsWith, stringFilter.Title.Compare.Value) || stringFilter.Title.StartsWith.IsNullOrEmpty());
            }
        }
    }

    [Benchmark]
    public void StringFilter_AutoFilterer()
    {
        var query = books.ApplyFilter(stringFilter);
    }

    [Benchmark]
    public void MinAndMax_Lambda()
    {
        var query = books.AsQueryable();
        if (rangeFilter.TotalPage.Min != null && rangeFilter.TotalPage.Max != null)
        {
            if (rangeFilter.CombineWith == CombineType.And)
            {
                query = query.Where(x => x.TotalPage <= rangeFilter.TotalPage.Max && x.TotalPage >= rangeFilter.TotalPage.Min);
            }
            else
            {
                query = query.Where(x => x.TotalPage <= rangeFilter.TotalPage.Max || x.TotalPage >= rangeFilter.TotalPage.Min);
            }
        }
        else
        {
            if (rangeFilter.TotalPage.Min != null)
            {
                query = query.Where(x => x.TotalPage >= rangeFilter.TotalPage.Min);
            }
            else
            {
                query = query.Where(x => x.TotalPage <= rangeFilter.TotalPage.Max);
            }
        }
    }

    [Benchmark]
    public void MinAndMax_AutoFilterer()
    {
        var query = books.ApplyFilter(rangeFilter);
    }

    [Benchmark]
    public void ComplexFilter_Lambda()
    {
        var query = books.AsQueryable();
        if (complexFilter.CombineWith == CombineType.And)
        {
            query = query
                .Where(x =>
                    (complexFilter.Title == null || x.Title.Contains(complexFilter.Title, StringComparison.InvariantCultureIgnoreCase))
                    && (complexFilter.Author == null || x.Author.StartsWith(complexFilter.Author))
                    && (complexFilter.TotalPage.Min == null || complexFilter.TotalPage.Min.Value <= x.TotalPage)
                    && (complexFilter.TotalPage.Max == null || complexFilter.TotalPage.Max.Value >= x.TotalPage)
                    && (complexFilter.ReadCount == null || complexFilter.ReadCount.Value == x.ReadCount)
                    && (complexFilter.IsPublished == null || complexFilter.IsPublished == x.IsPublished)
                    );
        }
        else
        {
            query = query
              .Where(x =>
                  (complexFilter.Title == null || x.Title.Contains(complexFilter.Title, StringComparison.InvariantCultureIgnoreCase))
                  || (complexFilter.Author == null || x.Author.StartsWith(complexFilter.Author))
                  || (complexFilter.TotalPage.Min == null || complexFilter.TotalPage.Min.Value <= x.TotalPage)
                  || (complexFilter.TotalPage.Max == null || complexFilter.TotalPage.Max.Value >= x.TotalPage)
                  || (complexFilter.ReadCount == null || complexFilter.ReadCount.Value == x.ReadCount)
                  || (complexFilter.IsPublished == null || complexFilter.IsPublished == x.IsPublished)
                  );
        }
    }

    [Benchmark]
    public void ComplexFilter_AutoFilterer()
    {
        var query = books.ApplyFilter(complexFilter);
    }
}

public class BookFilter_ReadCount : FilterBase
{
    public int? ReadCount { get; set; }
}

public class BookFilter_RangeOfPage : FilterBase
{
    public Range<int> TotalPage { get; set; }
}

public class BookFilter_ReadCount_TotalPage : FilterBase
{
    public int? ReadCount { get; set; }
    public int? TotalPage { get; set; }
}
