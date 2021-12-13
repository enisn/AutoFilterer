#if LEGACY_NAMESPACE
using AutoFilterer.Enums;
#endif
using AutoFilterer;
using AutoFilterer.Attributes;
using AutoFilterer.Types;

namespace WebApplication.API.Dtos.Northwind;

public class ShipperFilter : PaginationFilterBase
{
    public int[] ShipperId { get; set; }

    [StringFilterOptions(StringFilterOption.Contains)]
    public string CompanyName { get; set; }

    [StringFilterOptions(StringFilterOption.Contains)]
    public string Phone { get; set; }
}
