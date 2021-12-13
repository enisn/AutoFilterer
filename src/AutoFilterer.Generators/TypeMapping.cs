using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFilterer.Types;

namespace AutoFilterer.Generators;

public class TypeMapping
{
    public static Dictionary<string, string> Mappings { get; } = new Dictionary<string, string>
    {
        { "sbyte", "Range<sbyte>" },
        { "sbyte?", "Range<sbyte>" },
        { "byte", "Range<byte>" },
        { "byte?", "Range<byte>" },
        { "short", "Range<short>" },
        { "short?", "Range<short>" },
        { "ushort", "Range<ushort>" },
        { "ushort?", "Range<short>" },
        { "int", "Range<int>" },
        { "int?", "Range<int>" },
        { "uint", "Range<uint>" },
        { "uint?", "Range<int>" },
        { "long", "Range<long>" },
        { "long?", "Range<long>" },
        { "ulong", "Range<ulong>" },
        { "ulong?", "Range<ulong>" },
        { "double", "Range<double>" },
        { "double?", "Range<double>" },
        { "float", "Range<float>" },
        { "float?", "Range<float>" },
        { "decimal", "Range<decimal>" },
        { "decimal?", "Range<decimal>" },
        // Special cases for some types:
        { "System.DateTime", "Range<System.DateTime>" },
        { "System.DateTime?", "Range<System.DateTime>" },
        { "System.TimeSpan", "Range<System.TimeSpan>" },
        { "System.TimeSpan?", "Range<System.TimeSpan>" },
    };
}
