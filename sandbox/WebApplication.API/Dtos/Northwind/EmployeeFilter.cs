#if LEGACY_NAMESPACE
using AutoFilterer.Enums;
#endif
using AutoFilterer;
using AutoFilterer.Attributes;
using AutoFilterer.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication.API.Models.Northwind;

namespace WebApplication.API.Dtos.Northwind;

public class EmployeeFilter : PaginationFilterBase
{
    public int[] EmployeeId { get; set; }

    [CompareTo(nameof(Employee.FirstName), nameof(Employee.LastName), nameof(Employee.Title), nameof(Employee.TitleOfCourtesy))]
    public string Search { get; set; }

    public Range<DateTime> BirthDate { get; set; }

    public Range<DateTime> HireDate { get; set; }

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
    public string HomePhone { get; set; }
    [StringFilterOptions(StringFilterOption.Contains)]
    public string Extension { get; set; }
    [StringFilterOptions(StringFilterOption.Contains)]
    public string Notes { get; set; }

    public EmployeeTerritoryFilter EmployeeTerritories { get; set; }
}
