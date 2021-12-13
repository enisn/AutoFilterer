using System;
using System.Linq;
using System.Reflection;

namespace AutoFilterer.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class PossibleSortingsAttribute : OrderingOptionsBaseAttribute
{
    public PossibleSortingsAttribute(params string[] propertyNames)
    {
        this.PropertyNames = propertyNames;
    }

    /// <summary>
    /// Generates an attribute from type and finds PropertyNames from Enum values or properties of class.
    /// </summary>
    /// <param name="typeMetaData">Enum or Class.</param>
    public PossibleSortingsAttribute(Type typeMetaData)
    {
        if (typeMetaData.IsEnum)
        {
            this.PropertyNames = Enum.GetNames(typeMetaData);
        }
        else
        {
            this.PropertyNames = typeMetaData
                                    .GetProperties()
                                    .Where(x => x.CanWrite && x.GetCustomAttribute<IgnoreFilterAttribute>() == null)
                                    .SelectMany(s => s.GetCustomAttribute<CompareToAttribute>()?.PropertyNames ?? new[] { s.Name })
                                    .ToArray();
        }
    }

    public string[] PropertyNames { get; }
}
