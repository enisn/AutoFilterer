using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication.API.Dtos;
using WebApplication.API.Models;
using WebApplication.API.Repository;
using AutoFilterer.Extensions;

namespace WebApplication.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly BooksRepository repository;

        public BooksController(BooksRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(typeof(Book[]), 200)]
        public IActionResult Get([FromQuery]BookFilter filter)
        {
            var result = repository.Books.ApplyFilter(filter).ToList();

            return Ok(result);
        }
    }
}
