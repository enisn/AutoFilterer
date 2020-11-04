using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoFilterer.Extensions;
using System.Threading.Tasks;
using WebApplication.API.Contexts.Contexts;
using WebApplication.API.Dtos.Northwind;
using Microsoft.EntityFrameworkCore;

namespace WebApplication.API.Controllers
{
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
            => Ok(dbContext.Categories.ApplyFilter(filter));

        [HttpGet]
        public IActionResult Customers([FromQuery] CustomerFilter filter)
            => Ok(dbContext.Customers.ApplyFilter(filter));

        [HttpGet]
        public IActionResult CustomerCustomerDemos([FromQuery] CustomerCustomerDemoFilter filter)
            => Ok(dbContext.CustomerCustomerDemos
                                .Include(i => i.Customer)
                                .Include(i => i.CustomerType)
                                .ApplyFilter(filter));

        [HttpGet]
        public IActionResult CustomerDemographics([FromQuery] CustomerDemographicFilter filter)
            => Ok(dbContext.CustomerDemographics.ApplyFilter(filter));

        [HttpGet]
        public IActionResult Employees([FromQuery] EmployeeFilter filter)
            => Ok(dbContext.Employees
                                .Include(i => i.EmployeeTerritories)
                                .ThenInclude(ti => ti.Territory)
                                .ApplyFilter(filter));
        [HttpGet]
        public IActionResult Orders([FromQuery] OrderFilter filter)
            => Ok(dbContext.Orders
                                .Include(i => i.OrderDetails)
                                .ThenInclude(ti => ti.Product)
                                .ApplyFilter(filter));

        [HttpGet]
        public IActionResult Products([FromQuery] ProductFilter filter)
            => Ok(dbContext.Products
                                .Include(i => i.Supplier)
                                .Include(i => i.Category)
                                .ApplyFilter(filter));

        [HttpGet]
        public IActionResult Regions([FromQuery] RegionFilter filter)
            => Ok(dbContext.Regions
                                .Include(i => i.Territories)
                                .ApplyFilter(filter));

        [HttpGet]
        public IActionResult Shippers([FromQuery] ShipperFilter filter)
            => Ok(dbContext.Shippers.ApplyFilter(filter));

        [HttpGet]
        public IActionResult Suppliers([FromQuery] SupplierFilter filter)
            => Ok(dbContext.Suppliers.ApplyFilter(filter));

        [HttpGet]
        public IActionResult Territories([FromQuery] TerritoryFilter filter)
            => Ok(dbContext.Territories.ApplyFilter(filter));
    }
}
