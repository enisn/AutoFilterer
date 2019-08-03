using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using AutoFilterer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication.API.Dtos
{
    public class BlogPaginationFilterDto : PaginationFilterBase
    {
        public int? CategoryId { get; set; }

        public Range<int> Priority { get; set; }

        [StringFilterOptions(StringFilterOption.Contains)]
        public string Title { get; set; }

        public bool? IsPublished { get; set; }

        public Range<DateTime> PublishDate { get; set; }
    }
}
