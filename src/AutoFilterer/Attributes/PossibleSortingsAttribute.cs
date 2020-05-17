using System;

namespace AutoFilterer.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PossibleSortingsAttribute : OrderingOptionsBaseAttribute
    {
        public PossibleSortingsAttribute(params string[] propertyNames)
        {
            this.PropertyNames = propertyNames;
        }

        public string[] PropertyNames { get; }
    }
}
