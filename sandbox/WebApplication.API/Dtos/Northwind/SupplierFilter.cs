#if LEGACY_NAMESPACE
using AutoFilterer.Enums;
#endif
using AutoFilterer;
using AutoFilterer.Attributes;
using AutoFilterer.Types;
using WebApplication.API.Models.Northwind;

namespace WebApplication.API.Dtos.Northwind;

public class SupplierFilter : PaginationFilterBase
{
    public int[] SupplierId { get; set; }

    /// <summary>
    /// Searches in **CompanyName**, **ContactName** and **ContactTitle**.
    /// </summary>
    [CompareTo(nameof(Supplier.CompanyName), nameof(Supplier.ContactName), nameof(Supplier.ContactTitle))]
    [StringFilterOptions(StringFilterOption.Contains)]
    public string Search { get; set; }

    [StringFilterOptions(StringFilterOption.Contains)]
    public string Address { get; set; }
    [StringFilterOptions(StringFilterOption.Contains)]
    public string City { get; set; }
    [StringFilterOptions(StringFilterOption.Contains)]
    public string Region { get; set; }
    [StringFilterOptions(StringFilterOption.Contains)]
    public string PostalCode { get; set; }
    [StringFilterOptions(StringFilterOption.Contains)]
    public string Country { get; set; }
    [StringFilterOptions(StringFilterOption.Contains)]
    public string Phone { get; set; }
    [StringFilterOptions(StringFilterOption.Contains)]
    public string Fax { get; set; }
    [StringFilterOptions(StringFilterOption.Contains)]
    public string HomePage { get; set; }
}
