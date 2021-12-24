using AutoFilterer.Types;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;
using System.Reflection;

namespace AutoFilterer.Swagger.OperationFilters;

public class InnerFilterPropertiesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var ignoredProps = GetIgnoredProperties();
        for (int i = 0; i < operation.Parameters.Count; i++)
        {
            var parameter = operation.Parameters[i];
            if (IsIgnored(parameter.Name, ignoredProps))
            {
                operation.Parameters.Remove(parameter);
                i--;
            }
        }
    }

    private static PropertyInfo[] GetIgnoredProperties() => typeof(PaginationFilterBase).GetProperties();

    private static bool IsIgnored(string propertyName, PropertyInfo[] properties)
    {
        return properties.Any(a => propertyName.IndexOf("." + a.Name, StringComparison.InvariantCultureIgnoreCase) > 0);
    }
}
