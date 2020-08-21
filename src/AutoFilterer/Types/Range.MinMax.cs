using AutoFilterer.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoFilterer.Types
{
    public partial class Range<T>
    {
        public static T1 GetMaxValue<T1>()
        {
            object maxValue = default(T1);
            TypeCode typeCode = Type.GetTypeCode(typeof(T1));
            switch (typeCode)
            {
                case TypeCode.Byte:
                    maxValue = byte.MaxValue;
                    break;
                case TypeCode.Char:
                    maxValue = char.MaxValue;
                    break;
                case TypeCode.DateTime:
                    maxValue = DateTime.MaxValue;
                    break;
                case TypeCode.Decimal:
                    maxValue = decimal.MaxValue;
                    break;
                case TypeCode.Double:
                    maxValue = decimal.MaxValue;
                    break;
                case TypeCode.Int16:
                    maxValue = short.MaxValue;
                    break;
                case TypeCode.Int32:
                    maxValue = int.MaxValue;
                    break;
                case TypeCode.Int64:
                    maxValue = long.MaxValue;
                    break;
                case TypeCode.SByte:
                    maxValue = sbyte.MaxValue;
                    break;
                case TypeCode.Single:
                    maxValue = float.MaxValue;
                    break;
                case TypeCode.UInt16:
                    maxValue = ushort.MaxValue;
                    break;
                case TypeCode.UInt32:
                    maxValue = uint.MaxValue;
                    break;
                case TypeCode.UInt64:
                    maxValue = ulong.MaxValue;
                    break;
                default:
                    maxValue = default(T1); // Set as default value.
                    break;
            }
            return (T1)maxValue;
        }

        public static T1 GetMinValue<T1>()
        {
            object maxValue = default(T1);
            TypeCode typeCode = Type.GetTypeCode(typeof(T1));
            switch (typeCode)
            {
                case TypeCode.Byte:
                    maxValue = byte.MinValue;
                    break;
                case TypeCode.Char:
                    maxValue = char.MinValue;
                    break;
                case TypeCode.DateTime:
                    maxValue = DateTime.MinValue;
                    break;
                case TypeCode.Decimal:
                    maxValue = decimal.MinValue;
                    break;
                case TypeCode.Double:
                    maxValue = double.MinValue;
                    break;
                case TypeCode.Int16:
                    maxValue = short.MinValue;
                    break;
                case TypeCode.Int32:
                    maxValue = int.MinValue;
                    break;
                case TypeCode.Int64:
                    maxValue = long.MinValue;
                    break;
                case TypeCode.SByte:
                    maxValue = sbyte.MinValue;
                    break;
                case TypeCode.Single:
                    maxValue = float.MinValue;
                    break;
                case TypeCode.UInt16:
                    maxValue = ushort.MinValue;
                    break;
                case TypeCode.UInt32:
                    maxValue = uint.MinValue;
                    break;
                case TypeCode.UInt64:
                    maxValue = ulong.MinValue;
                    break;
                default:
                    maxValue = default(T1); // set default value
                    break;
            }

            return (T1)maxValue;
        }
    }
}
