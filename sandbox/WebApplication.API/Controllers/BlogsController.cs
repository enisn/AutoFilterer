using System;
using System.Linq;
using AutoFilterer.Enums;
using AutoFilterer.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication.API.Dtos;
using WebApplication.API.Models;
using WebApplication.API.Repository;

namespace WebApplication.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        // api/Blogs
        // api/Blogs?title=hello
        // api/Blogs?title=hello
        // api/Blogs?isPublished=False
        // api/Blogs?priority.min=2
        [HttpGet]
        public IActionResult Get([FromQuery]BlogFilterDto filter)
        {
            var db = new BlogDummyData();

            var list = db.Blogs.ApplyFilter(filter);

            return Ok(list);
        }

        // api/Blogs
        // api/Blogs?page=1
        // api/Blogs?page=2&perPage=4
        // api/Blogs?title=hello
        // api/Blogs?priority.min=5
        [HttpGet("paged")]
        public IActionResult GetWithPages([FromQuery]BlogPaginationFilterDto filter)
        {
            var db = new BlogDummyData();

            var list = db.Blogs.OrderBy(o => o.PublishDate).ApplyFilter(filter);

            return Ok(list);
        }
    }
}