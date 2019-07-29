using AutoFilterer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace AutoFilterer.Types
{
    public class Range<T> : IRange<T>, IRange, IEquatable<string>, IFormattable
        where T : struct, IComparable
    {
        public Range()
        {

        }
        public Range(string value)
        {
            var parsed = Parse<T>(value);
            this.Min = parsed.Min;
            this.Max = parsed.Max;
        }

        public Range(T? min, T? max) : this()
        {
            Min = min;
            Max = max;
        }

        public T? Min { get; set; }
        public T? Max { get; set; }

        IComparable IRange.Min => Min;
        IComparable IRange.Max => Max;

        public static implicit operator Range<T>(string val)
        {
            return Parse<T>(val);
        }

        public static Range<T> Parse<T>(string value) where T : struct, IComparable
        {
            var splitted = value.Split(' ');

            return new Range<T>(
                        splitted[0] == null || splitted[0] == "-" ? default(T) : (T)Convert.ChangeType(splitted[0], typeof(T)),
                        splitted[1] == null || splitted[1] == "-" ? default(T) : (T)Convert.ChangeType(splitted[1], typeof(T))
                        );
        }

        public static explicit operator string(Range<T> val)
        {
            return val.ToString();
        }

        public override string ToString()
        {
            return $"{this.Min?.ToString() ?? "-"} {this.Max?.ToString() ?? "-"}";
        }

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
                    maxValue = default(T1);//set default value
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

        public Expression<Func<TEntity, bool>> BuildExpression<TEntity>(PropertyInfo property, object value)
        {
            var parameter = Expression.Parameter(property.DeclaringType, property.Name);

            var comparison = GetRangeComparison();

            return Expression.Lambda<Func<TEntity, bool>>(comparison, parameter);

            BinaryExpression GetRangeComparison()
            {
                BinaryExpression minExp = default, maxExp = default;

                if (Min != null)
                {
                    minExp = Expression.GreaterThanOrEqual(
                               Expression.Property(parameter, property.Name),
                               Expression.Constant(Min));
                    if (Max == null)
                        return minExp;
                }

                if (Max != null)
                {
                    maxExp = Expression.LessThanOrEqual(
                                Expression.Property(parameter, property.Name),
                                Expression.Constant(Max));
                    if (Min == null)
                        return maxExp;
                }

                return Expression.And(minExp, maxExp);
            }
        }

        public bool Equals(string other)
        {
            return this.ToString() == other;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return this.ToString();
        }
    }
}
