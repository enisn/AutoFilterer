using AutoFilterer.Attributes;
using AutoFilterer.Types;

namespace AutoFilterer.Tests.Environment.Dtos
{
    public class PreferencesFilterBase : FilterBase
    {
        public bool? IsTwoFactorEnabled { get; set; }

        [StringFilterOptions(StringFilterOption.EndsWith)]
        public string GivenName { get; set; }

        public Range<int> SecurityLevel { get; set; }
    }
}
