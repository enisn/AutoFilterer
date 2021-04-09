using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoFilterer.Extensions;
using System.Text;
using System.Threading.Tasks;
using WebApplication.API.Repository;

namespace WebApplication.API.Controllers
{
    [Route("api/[controller]")]
    public class BooksGeneratorsController : BaseController
    {
        private readonly BooksRepository repository;

        public BooksGeneratorsController(BooksRepository repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// This endpoint uses auto genererated filter object with `AutoFilterer.Generators` package.
        /// 
        /// You can jump to source code:
        /// - [BooksGeneratorsController.cs](https://github.com/enisn/AutoFilterer/blob/master/sandbox/WebApplication.API/Controllers/BooksGeneratorsController.cs#L32)
        /// - [Book.cs](https://github.com/enisn/AutoFilterer/blob/master/sandbox/WebApplication.API/Models/Book.cs#L6)
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get([FromQuery] WebApplication.BookFilter filter)
        {
            var result = repository.Books.ApplyFilter(filter).ToList();

            return Ok(result);
        }
    }
}
