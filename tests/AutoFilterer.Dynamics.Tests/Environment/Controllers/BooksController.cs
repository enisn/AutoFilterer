using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AutoFilterer.Dynamics.Tests.Environment.Controllers;

[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    [HttpGet]
    public IActionResult Get(DynamicFilter filter)
    {
        return Ok(filter.ToDictionary(k => k.Key, v => v.Value));
    }
}
