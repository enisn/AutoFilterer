using System;

[AttributeUsage(AttributeTargets.Class)]
public class AutoFilterDtoAttribute : Attribute
{
    public AutoFilterDtoAttribute()
    {
    }

    public AutoFilterDtoAttribute(string @namespace)
    {
        Namespace = @Namespace;
    }
    
    public string Namespace { get; }
}
