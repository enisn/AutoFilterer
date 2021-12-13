using AutoFilterer.Swagger.OperationFilters;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AutoFilterer.Swagger;

public static class Startup
{
    public static SwaggerGenOptions UseAutoFiltererParameters(this SwaggerGenOptions options)
    {
        options.OperationFilter<OrderableEnumOperationFilter>();
        options.OperationFilter<InnerFilterPropertiesOperationFilter>();
        return options;
    }
}
