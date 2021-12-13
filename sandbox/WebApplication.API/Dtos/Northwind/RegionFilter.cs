#if LEGACY_NAMESPACE
using AutoFilterer.Enums;
#endif
using AutoFilterer;
using AutoFilterer.Attributes;
using AutoFilterer.Types;

namespace WebApplication.API.Dtos.Northwind;

public class RegionFilter : PaginationFilterBase
{
    public int[] RegionId { get; set; }

    [StringFilterOptions(StringFilterOption.Contains)]
    public string RegionDescription { get; set; }
}
