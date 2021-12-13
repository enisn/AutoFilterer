using AutoFilterer.Extensions;
using AutoFilterer.Tests.Core;
using AutoFilterer.Tests.Environment.Dtos;
using AutoFilterer.Tests.Environment.Models;
#if LEGACY_NAMESPACE
using AutoFilterer.Enums;
#endif
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace AutoFilterer.Tests;

public class PerformanceTests
{
    [Theory, AutoMoqData(count: 1024)]
    public void BuildExpression_TotalPageWithAnd_SholdMatchCount(List<Book> data, BookFilter_OperatorFilter_TotalPage filter)
    {
        // Arrange
        filter.CombineWith = CombineType.And;
        Stopwatch sw = new Stopwatch();

        // Act
        sw.Restart();
        var classicalQuery = GetAndQuery(data.AsQueryable(), filter);
        var classicalResult = classicalQuery.ToList();
        sw.Stop();
        var classicalElapsed = sw.ElapsedTicks;
        Console.WriteLine(classicalElapsed);

        sw.Start();
        var query = data.AsQueryable().ApplyFilter(filter);
        var result = query.ToList();
        sw.Stop();
        var applyFilterElapsed = sw.ElapsedTicks;
        Console.WriteLine(applyFilterElapsed);

        // Assert
        Assert.True((classicalElapsed * 1.5f) > applyFilterElapsed); // Can't be slower than 1.5 times.

        // Local functions
        IQueryable<Book> GetAndQuery(IQueryable<Book> query, BookFilter_OperatorFilter_TotalPage f)
        {
            if (f.TotalPage.Eq != null)
                query = query.Where(x => x.TotalPage == f.TotalPage.Eq);

            if (f.TotalPage.Not != null)
                query = query.Where(x => x.TotalPage != f.TotalPage.Not);

            if (f.TotalPage.Gt != null)
                query = query.Where(x => x.TotalPage > f.TotalPage.Gt);

            if (f.TotalPage.Gte != null)
                query = query.Where(x => x.TotalPage >= f.TotalPage.Gte);

            if (f.TotalPage.Lt != null)
                query = query.Where(x => x.TotalPage < f.TotalPage.Lt);

            if (f.TotalPage.Lte != null)
                query = query.Where(x => x.TotalPage <= f.TotalPage.Lt);

            return query;
        }
    }

    [Theory, AutoMoqData(count: 1024)]
    public void BuildExpression(List<Book> data, BookFilter_MultiplePropertyWithOrDto filter)
    {
        // Arrange
        Stopwatch sw = new Stopwatch();

        // AutoFilterer Way
        sw.Restart();
        var aQuery = data.AsQueryable().ApplyFilter(filter);
        var aResult = aQuery.ToList();
        sw.Stop();
        var aElapsed = sw.ElapsedTicks;
        Console.WriteLine("AutoFilterer Elapsed: " + aElapsed);

        // Classical Way
        sw.Start();
        var cQuery = data.AsQueryable();
        if (filter.CombineWith == CombineType.And)
        {
            cQuery = cQuery.Where(x => (filter.Query != null && x.Title.Contains(filter.Query)) && (filter.Query != null && x.Author.Contains(filter.Query)));
        }
        else
        {
            cQuery = cQuery.Where(x => (filter.Query != null && x.Title.Contains(filter.Query)) || (filter.Query != null || x.Author.Contains(filter.Query)));
        }
        var cResult = cQuery.ToList();
        sw.Stop();
        var cElapsed = sw.ElapsedTicks;
        Console.WriteLine("Classical Elapsed: " + cElapsed);

        var ratio = cElapsed / (float)aElapsed;
        Console.WriteLine("Classical / AutoFilterer : " + ratio);

        // Assert
        Assert.True(ratio > 1.2f);
    }
}
