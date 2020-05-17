using System;

namespace AutoFilterer.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class OrderingOptionsBaseAttribute : Attribute
    {
    }
}
