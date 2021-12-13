#if LEGACY_NAMESPACE
using AutoFilterer.Enums;
#endif
using AutoFilterer;
using AutoFilterer.Attributes;
using AutoFilterer.Types;
using System;
using WebApplication.API.Models.Northwind;

namespace WebApplication.API.Dtos.Northwind;

public class OrderFilter : PaginationFilterBase
{
    public int[] OrderId { get; set; }
    public string[] CustomerId { get; set; }
    public int? EmployeeId { get; set; }
    public Range<DateTime> OrderDate { get; set; }
    public Range<DateTime> RequiredDate { get; set; }
    public Range<DateTime> ShippedDate { get; set; }
    public int? ShipVia { get; set; }
    public Range<float> Freight { get; set; }

    [CompareTo(nameof(Order.ShipName), nameof(Order.ShipAddress), nameof(Order.ShipCity), nameof(Order.ShipCountry), nameof(Order.ShipRegion), nameof(Order.ShipPostalCode))]
    [StringFilterOptions(StringFilterOption.Contains)]
    public string ShipSearch { get; set; }
}
