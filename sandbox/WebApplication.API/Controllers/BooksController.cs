using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication.API.Models;
using WebApplication.API.Repository;
using AutoFilterer.Extensions;

namespace WebApplication.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : BaseController
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
    /// This API is made with following 2 file with writing a couple lines of code:
    /// 
    /// - [Controller source-code](https://github.com/enisn/AutoFilterer/blob/master/sandbox/WebApplication.API/Controllers/BooksController.cs#L39)
    /// 
    /// - [BookFilter source-code](https://github.com/enisn/AutoFilterer/blob/master/sandbox/WebApplication.API/Dtos/BookFilter.cs)
    /// </remarks>
    /// <param name="filter"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(Book[]), 200)]
    public IActionResult Get([FromQuery] Dtos.BookFilter filter)
    {
        var result = repository.Books.ApplyFilter(filter);

        return Ok(result);
    }
}
