using AutoFilterer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Tests.Environment.Dtos;

public class PreferencesFilter_ArraySearchWithoutAttribute : FilterBase
{
    public int[] SecurityLevel { get; set; }
}
