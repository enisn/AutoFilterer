#if LEGACY_NAMESPACE
using AutoFilterer.Enums;
#else
using AutoFilterer;
#endif
using AutoFilterer.Attributes;
using AutoFilterer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Tests.Environment.Dtos;

public class PreferencesFilterBase : FilterBase
{
    public bool? IsTwoFactorEnabled { get; set; }

    [StringFilterOptions(StringFilterOption.EndsWith)]
    public string GivenName { get; set; }

    public Range<int> SecurityLevel { get; set; }
}
