using System;

[AttributeUsage(AttributeTargets.Class)]
public class GenerateAutoFilterAttribute : Attribute
{
    public GenerateAutoFilterAttribute()
    {
    }

    public GenerateAutoFilterAttribute(string @namespace)
    {
        Namespace = @Namespace;
    }
    
    public string Namespace { get; }
}
