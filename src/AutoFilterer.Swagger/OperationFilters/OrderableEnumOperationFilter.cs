using AutoFilterer.Abstractions;
using AutoFilterer.Attributes;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using System.Reflection;

namespace AutoFilterer.Swagger.OperationFilters
{
    public class OrderableEnumOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            foreach (var item in operation.Parameters)
            {
                var param = context.ApiDescription.ParameterDescriptions.FirstOrDefault(x => x.Name == item.Name);

                if (typeof(IOrderable).IsAssignableFrom(param.ModelMetadata.ContainerType) && param.Name == nameof(IOrderable.Sort))
                {
                    foreach (var prop in param.ModelMetadata.ContainerType.GetProperties())
                        if (!prop.HasAttribute<IgnoreFilterAttribute>())
                        {
                            var compareTo = prop.GetCustomAttribute<CompareToAttribute>();
                            if (compareTo != null)
                            {
                                foreach (var propertyName in compareTo.PropertyNames)                                
                                    item.Schema.Enum.Add(new OpenApiString(propertyName));                                
                            }
                            else
                            {
                                item.Schema.Enum.Add(new OpenApiString(prop.Name));
                            }
                        }
                }
            }
        }
    }
}
