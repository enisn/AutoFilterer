using AutoFilterer.Types;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFilterer.Attributes;

namespace WebApplication.API.Dtos.Northwind
{
    public class ShipperFilter : PaginationFilterBase
    {
        public int[] ShipperId { get; set; }

        [StringFilterOptions(StringFilterOption.Contains)]
        public string CompanyName { get; set; }

        [StringFilterOptions(StringFilterOption.Contains)]
        public string Phone { get; set; }
    }
}
