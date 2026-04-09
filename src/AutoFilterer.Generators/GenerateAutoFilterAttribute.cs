using System;

[AttributeUsage(AttributeTargets.Class)]
public class GenerateAutoFilterAttribute : Attribute
{
    public GenerateAutoFilterAttribute()
    {
    }

    public GenerateAutoFilterAttribute(string @namespace)
    {
        Namespace = @namespace;
    }

    public string Namespace { get; }

    public string BaseClass { get; set; }

    public bool UseStringFilter { get; set; }

    public bool UseRangeForNumbers { get; set; } = true;

    public bool UseRangeForDates { get; set; } = true;

    public bool GenerateForEnumProperties { get; set; } = true;
}