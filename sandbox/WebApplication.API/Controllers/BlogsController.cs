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

            var query = db.Blogs.OrderByDescending(o => o.PublishDate);

            var list = filter.ApplyFilterTo(query).ToList();

            return Ok(list);
        }
    }
}