#if LEGACY_NAMESPACE
using AutoFilterer.Enums;
#endif
using AutoFilterer;
using AutoFilterer.Attributes;
using AutoFilterer.Types;
using WebApplication.API.Models.Northwind;

namespace WebApplication.API.Dtos.Northwind;

public class CustomerFilter : PaginationFilterBase
{
    [ArraySearchFilter]
    public string[] CustomerId { get; set; }

    [CompareTo(nameof(Customer.CompanyName), nameof(Customer.ContactTitle), nameof(Customer.ContactName))]
    [StringFilterOptions(StringFilterOption.Contains)]
    public string Search { get; set; }

    [StringFilterOptions(StringFilterOption.Contains)]
    public string Address { get; set; }

    [StringFilterOptions(StringFilterOption.Contains)]
    public string City { get; set; }

    [StringFilterOptions(StringFilterOption.Contains)]
    public string Region { get; set; }

    [StringFilterOptions(StringFilterOption.Equals)]
    public string PostalCode { get; set; }

    [StringFilterOptions(StringFilterOption.Contains)]
    public string Country { get; set; }

    [StringFilterOptions(StringFilterOption.Equals)]
    public string Phone { get; set; }

    [StringFilterOptions(StringFilterOption.Equals)]
    public string Fax { get; set; }
}
