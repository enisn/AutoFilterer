using AutoFilterer.Swagger.OperationFilters;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Swagger
{
    public static class Startup
    {
        public static SwaggerGenOptions AddAutoFilterer(this SwaggerGenOptions options)
        {
            options.OperationFilter<OrderableEnumOperationFilter>();
            return options;
        }
    }
}
