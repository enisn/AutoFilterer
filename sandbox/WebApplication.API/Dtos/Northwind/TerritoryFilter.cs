#if LEGACY_NAMESPACE
using AutoFilterer.Enums;
#endif
using AutoFilterer;
using AutoFilterer.Attributes;
using AutoFilterer.Types;

namespace WebApplication.API.Dtos.Northwind;

public class TerritoryFilter : PaginationFilterBase
{
    public string[] TerritoryId { get; set; }
    [StringFilterOptions(StringFilterOption.Contains)]
    public string TerritoryDescription { get; set; }
}
