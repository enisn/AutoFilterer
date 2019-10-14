using AutoFilterer.Attributes;
using AutoFilterer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Tests.Dtos
{
    public class BookFilterBase : FilterBase
    {
        [StringFilterOptions(Enums.StringFilterOption.Contains)]
        public string Title { get; set; }
        [StringFilterOptions(Enums.StringFilterOption.StartsWith)]
        public string Author { get; set; }
        public Range<int> TotalPage { get; set; }
        public int? ReadCount { get; set; }
        public bool? IsPublished { get; set; }
    }
}
