using AutoFilterer.Attributes;
using AutoFilterer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication.API.Dtos.Northwind;

public class CustomerCustomerDemoFilter : PaginationFilterBase
{
    public string[] CustomerId { get; set; }
    public string[] CustomerTypeId { get; set; }
    public CustomerFilter Customer { get; set; }
    public CustomerDemographicFilter CustomerType { get; set; }
}
