using System;

namespace AutoFilterer.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
public abstract class OrderingOptionsBaseAttribute : Attribute
{
}
