using AutoFilterer.Attributes;
using AutoFilterer.Tests.Environment.Models;
using AutoFilterer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Tests.Environment.Dtos
{
    public class BookFilter_CompareToAs_ToLowerContains : FilterBase
    {
        [CompareToAs(typeof(ToLowerContainsComparisonAttribute), nameof(Book.Title), nameof(Book.Author))]
        public string Query { get; set; }
    }
}
