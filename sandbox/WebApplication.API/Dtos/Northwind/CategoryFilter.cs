using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using AutoFilterer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication.API.Models.Northwind;

namespace WebApplication.API.Dtos.Northwind
{
    public class CategoryFilter : PaginationFilterBase
    {
        [ArraySearchFilter]
        public int[] CategoryId { get; set; }

        [CompareTo(nameof(Category.CategoryName), nameof(Category.Description))]
        [StringFilterOptions(StringFilterOption.Contains)]
        public string Search { get; set; }
    }
}
