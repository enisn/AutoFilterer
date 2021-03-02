using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Generators
{
    public class TypeMapping
    {
        public static Dictionary<string, string> Mappings { get; } = new Dictionary<string, string>
        {
            { "int", "Range<int>" },
            { "int?", "Range<int>" },
            { "long", "Range<long>" },
            { "long?", "Range<long>" },
            { "DateTime", "Range<DateTime>" },
        };
    }
}
