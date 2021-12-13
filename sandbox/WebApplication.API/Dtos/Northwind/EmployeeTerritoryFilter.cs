using AutoFilterer.Attributes;

using AutoFilterer.Types;

namespace WebApplication.API.Dtos.Northwind;

public class EmployeeTerritoryFilter : FilterBase
{
    public string[] TerritoryId { get; set; }
}
