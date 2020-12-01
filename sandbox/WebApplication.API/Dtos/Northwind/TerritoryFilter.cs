using AutoFilterer.Attributes;

using AutoFilterer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication.API.Dtos.Northwind
{
    public class TerritoryFilter : PaginationFilterBase
    {
        public string[] TerritoryId { get; set; }
        [StringFilterOptions(StringFilterOption.Contains)]
        public string TerritoryDescription { get; set; }
    }
}
