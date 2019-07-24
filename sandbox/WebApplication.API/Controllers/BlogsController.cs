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
        // api/Blogs
        // api/Blogs?page=1
        // api/Blogs?page=2&perPage=4
        // api/Blogs?title=hello
        // api/Blogs?title=hello
        // api/Blogs?isPublished=False
        // api/Blogs?priority.min=2

        [HttpGet]
        public IActionResult Get([FromQuery]BlogFilterDto filter)
        {
            var db = new BlogDummyData();

            var query = db.Blogs.OrderByDescending(o => o.PublishDate);

            var list = filter.ApplyFilterTo(query).ToList();

            return Ok(list);
        }
    }
}