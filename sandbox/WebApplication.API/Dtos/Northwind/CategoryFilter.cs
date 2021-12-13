#if LEGACY_NAMESPACE
using AutoFilterer.Enums;
#endif
using AutoFilterer;
using AutoFilterer.Attributes;
using AutoFilterer.Types;
using WebApplication.API.Models.Northwind;

namespace WebApplication.API.Dtos.Northwind;

public class CategoryFilter : PaginationFilterBase
{
    [ArraySearchFilter]
    public int[] CategoryId { get; set; }

    [CompareTo(nameof(Category.CategoryName), nameof(Category.Description))]
    [StringFilterOptions(StringFilterOption.Contains)]
    public string Search { get; set; }
}
