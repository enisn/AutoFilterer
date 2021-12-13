using AutoFilterer.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WebApplication.API.Repository;

namespace WebApplication.API.Controllers;

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
    /// </summary>
    /// <remarks>
    /// You can jump to source code:
    /// - [BooksGeneratorsController.cs](https://github.com/enisn/AutoFilterer/blob/master/sandbox/WebApplication.API/Controllers/BooksGeneratorsController.cs#L32)
    /// - [Book.cs](https://github.com/enisn/AutoFilterer/blob/master/sandbox/WebApplication.API/Models/Book.cs#L6)
    /// <param name="filter"></param>
    /// </remarks>
    /// <returns></returns>
    [HttpGet]
    public IActionResult Get([FromQuery] WebApplication.BookFilter filter)
    {
        var result = repository.Books.ApplyFilter(filter).ToList();

        return Ok(result);
    }
}
