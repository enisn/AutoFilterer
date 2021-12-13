#if LEGACY_NAMESPACE
using AutoFilterer.Enums;
#endif
using AutoFilterer;
using AutoFilterer.Attributes;
using AutoFilterer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication.API.Dtos.Northwind;

public class CustomerDemographicFilter : PaginationFilterBase
{
    public string[] CustomerTypeId { get; set; }

    [StringFilterOptions(StringFilterOption.Contains)]
    public string CustomerDesc { get; set; }
}
