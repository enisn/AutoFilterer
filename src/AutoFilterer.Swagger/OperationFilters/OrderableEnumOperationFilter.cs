using AutoFilterer.Abstractions;
using AutoFilterer.Attributes;
using AutoFilterer.Types;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;
using System.Reflection;

namespace AutoFilterer.Swagger.OperationFilters;

public class OrderableEnumOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        foreach (var parameter in operation.Parameters)
        {
            var parameterDescription = GetParameterDescription(context, parameter);

            if (IsOrderableAndSortParameter(parameterDescription))
            {
                var possibleSortings = parameterDescription.ModelMetadata.ContainerType.GetCustomAttribute<PossibleSortingsAttribute>();
                if (possibleSortings != null)
                {
                    foreach (var propertyName in possibleSortings.PropertyNames)
                        parameter.Schema.Enum.Add(new OpenApiString(propertyName));
                    break;
                }

                foreach (var prop in parameterDescription.ModelMetadata.ContainerType
                    .GetProperties().Where(x => !x.HasAttribute<IgnoreFilterAttribute>()))
                {
                    AddPropertyAsEnum(parameter, prop);
                }
            }
        }
    }

    private static ApiParameterDescription GetParameterDescription(OperationFilterContext context, OpenApiParameter item)
    {
        return context.ApiDescription.ParameterDescriptions.FirstOrDefault(x => x.Name.Equals(item.Name, StringComparison.InvariantCultureIgnoreCase));
    }

    private static bool IsOrderableAndSortParameter(Microsoft.AspNetCore.Mvc.ApiExplorer.ApiParameterDescription param)
    {
        return param?.ModelMetadata?.ContainerType != null
            && typeof(IOrderable).IsAssignableFrom(param.ModelMetadata.ContainerType)
            && param.Name == nameof(IOrderable.Sort);
    }

    private static void AddPropertyAsEnum(OpenApiParameter item, PropertyInfo prop, string aggragatedName = default)
    {
        var compareTo = prop.GetCustomAttribute<CompareToAttribute>();
        if (compareTo != null)
        {
            foreach (var propertyName in compareTo.PropertyNames)
                if (IsValidPropertyToOrder(propertyName))
                    item.Schema.Enum.Add(new OpenApiString((aggragatedName + "." + propertyName).Trim('.')));
        }
        else if (typeof(IFilter).IsAssignableFrom(prop.PropertyType))
        {
            foreach (var innerProp in prop.PropertyType.GetProperties())
                if (IsValidPropertyToOrder(innerProp.Name))
                    AddPropertyAsEnum(item, innerProp, (aggragatedName + "." + prop.Name).Trim('.'));
        }
        else if (IsValidPropertyToOrder(prop.Name))
        {
            item.Schema.Enum.Add(new OpenApiString((aggragatedName + "." + prop.Name).Trim('.')));
        }
    }

    private static bool IsValidPropertyToOrder(string propertyName)
        => !typeof(PaginationFilterBase).GetProperties().Any(a => a.Name == propertyName);
}
