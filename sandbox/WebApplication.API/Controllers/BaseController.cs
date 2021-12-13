using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication.API.Controllers;

public class BaseController : ControllerBase
{
    public OkObjectResult Ok<T>([ActionResultObjectValue] IQueryable<T> query)
    {
        var queryAsString = query.Expression.ToString();
        var indexa = queryAsString.IndexOf(").");
        if (indexa < 0)
            indexa = int.MaxValue;

        var indexb = queryAsString.IndexOf("].");
        if (indexb < 0)
            indexb = int.MaxValue;

        var index = Math.Min(indexa, indexb) + 1;
        return base.Ok(new
        {
            query = queryAsString[index..],
            data = query.ToList()
        });
    }
}
