using AutoFilterer.Attributes;
using AutoFilterer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Tests.Envirorment.Dtos
{
    public class PreferencesFilterBase : FilterBase
    {
        public bool? IsTwoFactorEnabled { get; set; }

        [StringFilterOptions(Enums.StringFilterOption.EndsWith)]
        public string GivenName { get; set; }

        public Range<int> SecurityLevel { get; set; }
    }
}
