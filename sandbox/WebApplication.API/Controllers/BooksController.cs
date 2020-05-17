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
        /// <summary>
        /// Fitler books.
        /// </summary>
        /// <remarks>
        /// 
        /// - You can see Controller source-code from [here](https://github.com/enisn/AutoFilterer/blob/master/sandbox/WebApplication.API/Controllers/BooksController.cs#L29)
        /// 
        /// - You can see FilterBase source-code from [here](https://github.com/enisn/AutoFilterer/blob/master/sandbox/WebApplication.API/Dtos/BookFilter.cs)
        /// </remarks>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(Book[]), 200)]
        public IActionResult Get([FromQuery]BookFilter filter)
        {
            var result = repository.Books.ApplyFilter(filter).ToList();

            return Ok(result);
        }
    }
}
