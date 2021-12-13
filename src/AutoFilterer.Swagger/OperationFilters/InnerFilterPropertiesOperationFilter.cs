using AutoFilterer.Types;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Swagger.OperationFilters;

public class InnerFilterPropertiesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var ignoredProps = GetIgnoredProperties();
        for (int i = 0; i < operation.Parameters.Count; i++)
        {
            var item = operation.Parameters[i];
            if (IsIgnored(item.Name, ignoredProps))
            {
                operation.Parameters.Remove(item);
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
