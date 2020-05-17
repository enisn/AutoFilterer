using AutoFilterer.Tests.Envirorment.Statics;
using AutoFilterer.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using AutoFilterer.Tests.Envirorment.Models;
using AutoFilterer.Tests.Envirorment.Dtos;
using System.Diagnostics;
using System.Linq.Expressions;

namespace AutoFilterer.Tests
{
    public class PerformanceTests
    {
        [Theory, AutoMoqData(count: 1024)]
        public void BuildExpression_TotalPageWithAnd_SholdMatchCount(List<Book> data, BookFilter_OperatorFilter_TotalPage filter)
        {
            // Arrange
            filter.CombineWith = Enums.CombineType.And;
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
            if (filter.CombineWith == Enums.CombineType.And)
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

        [Theory, AutoMoqData(count: 64)]
        public void Test(List<Book> data)
        {
            var books = data.AsQueryable();
            var filter = new BookFilterBase { Title = "a" };
            Stopwatch sw = new Stopwatch();

            List<long> bResults = new List<long>();
            for (int i = 0; i < 1000; i++)
            {
                sw.Restart();
                var parameter = Expression.Parameter(typeof(Book), "x");
                var property = Expression.Property(parameter, "Title");

                var comparison = Expression.Or(
                        Expression.Equal(property, Expression.Constant(null)),
                        Expression.Equal(property, Expression.Constant(filter.Title))
                    );

                var queryB = books.Where(Expression.Lambda<Func<Book, bool>>(comparison, parameter));
                sw.Stop();
                bResults.Add(sw.ElapsedTicks);
            }
            var b = bResults.Sum() / (float)1000;

            List<long> aResults = new List<long>();
            for (int i = 0; i < 1000; i++)
            {
                sw.Restart();
                var queryA = books.Where(x => filter.Title == null || x.Title == filter.Title);
                sw.Stop();
                aResults.Add(sw.ElapsedTicks);
            }
            var a = aResults.Sum() / (float)1000;

            var ratio = a / b;

            Console.WriteLine("a: " + a);
            Console.WriteLine("b: " + b);

            // Assert
            Assert.True(ratio > 1.2f);
        }
    }
}
