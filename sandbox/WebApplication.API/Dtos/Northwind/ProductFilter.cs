using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using AutoFilterer.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication.API.Models.Northwind;

namespace WebApplication.API.Dtos.Northwind
{
    public class ProductFilter : PaginationFilterBase
    {
        public int[] ProductId { get; set; }

        [StringFilterOptions(StringFilterOption.Contains)]
        public string ProductName { get; set; }

        public int?[] SupplierId { get; set; }

        public int?[] CategoryId { get; set; }

        [StringFilterOptions(StringFilterOption.Contains)]
        public string QuantityPerUnit { get; set; }

        public Range<float> UnitPrice { get; set; }

        public Range<short> UnitsInStock { get; set; }

        public Range<short> UnitsOnOrder { get; set; }

        public Range<short> ReorderLevel { get; set; }

        public bool? Discontinued { get; set; }

        public CategoryFilter Category { get; set; }

        public SupplierFilter Supplier { get; set; }
    }
}
