using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Linq;

namespace WebApplication.API.Controllers;

/*
 * This controller is just for demonstration.
 * DO NOT INCLUDE THIS FILE IN YOUR PROJECT.
 */

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
