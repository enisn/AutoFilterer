using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Tests.Models
{
    public class Preferences
    {
        public Guid UserId { get; set; }
        public bool IsTwoFactorEnabled { get; set; }
        public string GivenName { get; set; }
        public int SecurityLevel { get; set; }
    }
}
