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

    public class FilterGeneratorTests
    {
        [Fact]
        public void ShouldBookFilterBeCreated()
        {
            // If there is no compile error. Everything is OK 👍
            Assert.True(typeof(AutoFilterer.Filters.BookFilter) != null);   
        }

        [Fact]
        public void ShouldTitleBeString()
        {
            var type = typeof(AutoFilterer.Filters.BookFilter);

            Assert.True(type.GetProperty(nameof(Book.Title)).PropertyType == typeof(string));
        }
    }
}
