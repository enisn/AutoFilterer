using AutoFilterer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace AutoFilterer.Types
{
    public partial class Range<T> : IRange<T>, IRange, IEquatable<string>, IFormattable
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

        public Expression BuildExpression(Expression body, PropertyInfo property, object value)
        {
            return GetRangeComparison();

            BinaryExpression GetRangeComparison()
            {
                BinaryExpression minExp = default, maxExp = default;

                if (Min != null)
                {
                    minExp = Expression.GreaterThanOrEqual(
                               Expression.Property(body, property.Name),
                               Expression.Constant(Min));
                    if (Max == null)
                        return minExp;
                }

                if (Max != null)
                {
                    maxExp = Expression.LessThanOrEqual(
                                Expression.Property(body, property.Name),
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
