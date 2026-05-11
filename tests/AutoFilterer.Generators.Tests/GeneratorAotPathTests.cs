using AutoFilterer.Generators.Tests.Environment.Dtos;
using System;
using System.Collections.Generic;
using Xunit;

namespace AutoFilterer.Generators.Tests;

public class GeneratorAotPathTests
{
    public static IEnumerable<object[]> ReflectionFreeFilterTypes()
    {
        yield return new object[] { typeof(BookFilter_Basic) };
        yield return new object[] { typeof(BookFilter_Range) };
        yield return new object[] { typeof(BookFilter_StringFilter_Advanced) };
        yield return new object[] { typeof(BookFilter_OperatorFilter_Advanced) };
        yield return new object[] { typeof(BookFilter_OrderableEdge) };
        yield return new object[] { typeof(Level1Filter_AllAnyAnyAnyAnyAnyAny) };
    }

    [Theory]
    [MemberData(nameof(ReflectionFreeFilterTypes))]
    public void GeneratedApplyFilter_DoesNotCallReflectionFallback(Type filterType)
    {
        Assert.False(GeneratedMethodInspector.CallsMethod(filterType, "ApplyFilterTo"));
    }
}
