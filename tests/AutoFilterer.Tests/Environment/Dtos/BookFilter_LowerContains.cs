using AutoFilterer.Attributes;
using AutoFilterer.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoFilterer.Tests.Environment.Dtos;

public class BookFilter_LowerContains : FilterBase
{
    [ToLowerContainsComparison]
    public string Title { get; set; }
}
