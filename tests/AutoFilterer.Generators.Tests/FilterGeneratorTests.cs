using AutoFilterer.Tests.Core;
using AutoFilterer.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AutoFilterer.Generators.Tests
{
    [AutoFilterDto]
    
    public class Book
    {
        public string Title { get; set; }
        public int? Year { get; set; }
        public int TotalPage { get; set; }
        public DateTime PublishTime { get; set; }
    }
    
    [AutoFilterDto("MyCustomNamespace")]
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
    }
}
