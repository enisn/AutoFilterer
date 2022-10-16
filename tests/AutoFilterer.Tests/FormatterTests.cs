using System;
using System.Linq;
using Xunit;

namespace AutoFilterer.Tests;

public class FormatterTests
{
    [Fact]
    public void QueryableSelect_Formatter_WorksAsExpressionFuncT()
    {
        var now = DateTime.Now;
        string format = "yyyy-MM-dd";
        
        var queryable = new[] { 1, 2, 3, 4, 5 }
            .Select(x => now.AddDays(x))
            .AsQueryable();

        var formatter = new Formatter<DateTime>(x => "Date: " + x.ToString(format));
        var values = queryable
            .Select(formatter)
            .ToList();

        var endsWith = now.ToString(format);
        
        Assert.All(values, x => x.EndsWith(endsWith));
    }
}