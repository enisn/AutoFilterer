using Microsoft.AspNetCore.Mvc;
using AutoFilterer.Extensions;
using WebApplication.API.Contexts.Contexts;
using WebApplication.API.Dtos.Northwind;
using Microsoft.EntityFrameworkCore;

namespace WebApplication.API.Controllers;

/// <summary>
/// This is sample comment line.
/// </summary>
[Route("api/[controller]/[action]")]
public class NorthwindController : BaseController
{
    private readonly NorthwindDbContext dbContext;

    public NorthwindController(NorthwindDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpGet]
    public IActionResult Categories([FromQuery] CategoryFilter filter)
    {
        return Ok(dbContext.Categories.ApplyFilter(filter));
    }

    [HttpGet]
    public IActionResult Customers([FromQuery] CustomerFilter filter)
    {
        return Ok(dbContext.Customers.ApplyFilter(filter));
    }

    [HttpGet]
    public IActionResult CustomerCustomerDemos([FromQuery] CustomerCustomerDemoFilter filter)
    {
        return Ok(dbContext.CustomerCustomerDemos
                                       .Include(i => i.Customer)
                                       .Include(i => i.CustomerType)
                                       .ApplyFilter(filter));
    }

    [HttpGet]
    public IActionResult CustomerDemographics([FromQuery] CustomerDemographicFilter filter)
    {
        return Ok(dbContext.CustomerDemographics.ApplyFilter(filter));
    }

    [HttpGet]
    public IActionResult Employees([FromQuery] EmployeeFilter filter)
    {
        return Ok(dbContext.Employees
                            .Include(i => i.EmployeeTerritories)
                            .ThenInclude(ti => ti.Territory)
                            .ApplyFilter(filter));
    }

    [HttpGet]
    public IActionResult Orders([FromQuery] OrderFilter filter)
    {
        return Ok(dbContext.Orders
                            .Include(i => i.OrderDetails)
                            .ThenInclude(ti => ti.Product)
                            .ApplyFilter(filter));
    }

    [HttpGet]
    public IActionResult Products([FromQuery] ProductFilter filter)
    {
        return Ok(dbContext.Products
                            .Include(i => i.Supplier)
                            .Include(i => i.Category)
                            .ApplyFilter(filter));
    }

    [HttpGet]
    public IActionResult Regions([FromQuery] RegionFilter filter)
    {
        return Ok(dbContext.Regions
                            .Include(i => i.Territories)
                            .ApplyFilter(filter));
    }

    [HttpGet]
    public IActionResult Shippers([FromQuery] ShipperFilter filter)
    {
        return Ok(dbContext.Shippers.ApplyFilter(filter));
    }

    [HttpGet]
    public IActionResult Suppliers([FromQuery] SupplierFilter filter)
    {
        return Ok(dbContext.Suppliers.ApplyFilter(filter));
    }

    [HttpGet]
    public IActionResult Territories([FromQuery] TerritoryFilter filter)
    {
        return Ok(dbContext.Territories.ApplyFilter(filter));
    }
}
