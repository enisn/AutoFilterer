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
    [Route("api/[controller]/[action]")]
    public class NorthwindController : ControllerBase
    {
        private readonly NorthwindDbContext dbContext;

        public NorthwindController(NorthwindDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Categories([FromQuery] CategoryFilter filter) 
            => Ok(await dbContext.Categories.ApplyFilter(filter).ToListAsync());

        [HttpGet]
        public async Task<IActionResult> Customers([FromQuery] CustomerFilter filter) 
            => Ok(await dbContext.Customers.ApplyFilter(filter).ToListAsync());

        [HttpGet]
        public async Task<IActionResult> CustomerCustomerDemos([FromQuery] CustomerCustomerDemoFilter filter) 
            => Ok(await dbContext.CustomerCustomerDemos.Include(i=> i.Customer).Include(i=> i.CustomerType).ApplyFilter(filter).ToListAsync());

        [HttpGet]
        public async Task<IActionResult> CustomerDemographics([FromQuery] CustomerDemographicFilter filter)
            => Ok(await dbContext.CustomerDemographics.ApplyFilter(filter).ToListAsync());

        [HttpGet]
        public async Task<IActionResult> Employees([FromQuery] EmployeeFilter filter)
            => Ok(await dbContext.Employees
                                .Include(i => i.EmployeeTerritories)
                                .ThenInclude(ti => ti.Employee)
                                .ApplyFilter(filter).ToListAsync());
        [HttpGet]
        public async Task<IActionResult> Orders([FromQuery] OrderFilter filter)
            => Ok(await dbContext.Orders
                                .Include(i => i.OrderDetails)
                                .ThenInclude(ti => ti.Product)
                                .ApplyFilter(filter).ToListAsync());

        [HttpGet]
        public async Task<IActionResult> Products([FromQuery] ProductFilter filter)
            => Ok(await dbContext.Products
                                .Include(i => i.Supplier)
                                .Include(i => i.Category)
                                .ApplyFilter(filter)
                                .ToListAsync());

        [HttpGet]
        public async Task<IActionResult> Regions([FromQuery] RegionFilter filter)
            => Ok(await dbContext.Regions
                                .Include(i => i.Territories)
                                .ApplyFilter(filter).ToListAsync());

        [HttpGet]
        public async Task<IActionResult> Shippers([FromQuery] ShipperFilter filter)
            => Ok(await dbContext.Shippers.ApplyFilter(filter).ToListAsync());

        [HttpGet]
        public async Task<IActionResult> Suppliers([FromQuery] SupplierFilter filter)
            => Ok(await dbContext.Suppliers.ApplyFilter(filter).ToListAsync());
    }
}
