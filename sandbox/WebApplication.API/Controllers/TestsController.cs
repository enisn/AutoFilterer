using AutoFilterer.Dynamics;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication.API.Dtos;

namespace WebApplication.API.Controllers;

[Route("/_api/[controller]")]
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class TestsController : ControllerBase
{
    [HttpGet("query-string-as-object")]
    public IActionResult GetQueryStringAsObject([FromQuery] DynamicFilter filter)
    {
        return Ok(filter);
    }
}
