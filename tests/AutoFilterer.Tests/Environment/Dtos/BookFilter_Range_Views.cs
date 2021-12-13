using AutoFilterer.Types;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Tests.Environment.Dtos;

public class BookFilter_Range_Views : FilterBase
{
    public Range<int> Views { get; set; }
}
