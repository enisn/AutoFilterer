using AutoFilterer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Tests.Environment.Dtos;

public class BookFilter_OperatorFilter_TotalPage : FilterBase
{
    public OperatorFilter<int> TotalPage { get; set; }
}
