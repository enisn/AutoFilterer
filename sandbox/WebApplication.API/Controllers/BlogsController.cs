using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication.API.Dtos;
using WebApplication.API.Repository;

namespace WebApplication.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        // /api/Blogs
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]BlogFilterDto filter)
        {
            var db = new BlogDummyData();

            var list = filter.ApplyFilterTo(db.Blogs).ToList();

            return Ok(list);
        }
    }
}