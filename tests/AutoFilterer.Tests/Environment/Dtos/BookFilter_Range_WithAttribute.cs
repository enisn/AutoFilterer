using AutoFilterer.Attributes;
using AutoFilterer.Types;

namespace AutoFilterer.Tests.Environment.Dtos;

public class BookFilter_Range_WithAttribute : FilterBase
{
    [CompareTo("TotalPage", "ReadCount")]
    public Range<int> Value { get; set; }
}
