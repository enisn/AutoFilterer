using AutoFilterer.Attributes;
using AutoFilterer.Types;
using System;

namespace AutoFilterer.Tests.Environment.Dtos
{
    public class BookFilterBase : FilterBase
    {
        [StringFilterOptions(StringFilterOption.Contains, StringComparison.InvariantCultureIgnoreCase)]
        public string Title { get; set; }
        [StringFilterOptions(StringFilterOption.StartsWith)]
        public string Author { get; set; }
        public Range<int> TotalPage { get; set; }
        public int? ReadCount { get; set; }
        public bool? IsPublished { get; set; }
    }
}
