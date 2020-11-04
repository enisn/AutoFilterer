using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using AutoFilterer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication.API.Dtos.Northwind
{
    public class RegionFilter : PaginationFilterBase
    {
        public int[] RegionId { get; set; }

        [StringFilterOptions(StringFilterOption.Contains)]
        public string RegionDescription { get; set; }

    }
}
