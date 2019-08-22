using AutoFilterer.Tests.Models;
using AutoFilterer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Tests.Dtos
{
    internal class UserFilterBase : FilterBase
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool? IsActive { get; set; }
        public BookFilterBase Books { get; set; }
        public PreferencesFilterBase Preferences { get; set; }
    }
}
