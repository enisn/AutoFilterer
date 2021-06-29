using AutoFilterer.Tests.Environment.Models;
using AutoFixture;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFilterer.Extensions;
using AutoFilterer.Tests.Environment.Dtos;
using AutoFilterer.Types;
using AutoFilterer.Enums;

namespace AutoFilterer.Benchmark
{
    [SimpleJob(RuntimeMoniker.HostProcess)]
    [DisassemblyDiagnoser]
    [MemoryDiagnoser]
    [ThreadingDiagnoser]
    public class AutoFiltererStartup
    {
        protected IQueryable<Book> books;
        protected BookFilter_ReadCount readCountFilter;
        protected BookFilter_RangeOfPage rangeFilter;

        [GlobalSetup]
        public void Setup()
        {
            var fixture = new Fixture();

            books = fixture.Create<List<Book>>().AsQueryable();
            readCountFilter = fixture.Create<BookFilter_ReadCount>();
            rangeFilter = fixture.Create<BookFilter_RangeOfPage>();
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
    }

    public class BookFilter_ReadCount : FilterBase
    {
        public int? ReadCount { get; set; }
    }

    public class BookFilter_RangeOfPage : FilterBase
    {
        public Range<int> TotalPage { get; set; }
    }
}
