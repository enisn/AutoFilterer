using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Abstractions
{
    public interface IEnumOrderable<TEnum> : IOrderable
        where TEnum : struct, Enum
    {
        new TEnum? Sort { get; }
    }
}
