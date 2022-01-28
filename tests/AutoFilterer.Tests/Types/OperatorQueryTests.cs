using AutoFilterer.Extensions;
using AutoFilterer.Tests.Core;
#if LEGACY_NAMESPACE
using AutoFilterer.Enums;
#else
using AutoFilterer;
#endif
using AutoFilterer.Tests.Environment.Dtos;
using AutoFilterer.Tests.Environment.Models;
using AutoFilterer.Types;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AutoFilterer.Tests.Types
{
    public class OperatorQueryTests
    {
        [Theory, AutoMoqData(count: 64)]
        public void BuildExpression_TotalPageWithAnd_SholdMatchCount(List<Book> data, BookFilter_OperatorFilter_TotalPage filter)
        {
            // Arrange
            filter.CombineWith = CombineType.And;

            // Act
            var query = data.AsQueryable().ApplyFilter(filter);
            var result = query.ToList();

            // Assert
            var f = filter;
            var actualResult = GetAndQuery(data.AsQueryable(), filter).ToList();
            Assert.Equal(actualResult.Count, result.Count);
            foreach (var item in actualResult)
                Assert.Contains(item, result);
            
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

                if (f.TotalPage.Lt!= null)
                    query = query.Where(x => x.TotalPage < f.TotalPage.Lt);

                if (f.TotalPage.Lte!= null)
                    query = query.Where(x => x.TotalPage <= f.TotalPage.Lt);

                if (f.TotalPage.IsNull != null)
                {
                    if (f.TotalPage.IsNull.Value)
                        query = query.Where(x => x.TotalPage == null);
                    else
                        query = query.Where(x => x.TotalPage != null);
                }
                
                if (f.TotalPage.IsNotNull != null)
                {
                    if (f.TotalPage.IsNotNull.Value)
                        query = query.Where(x => x.TotalPage != null);
                    else
                        query = query.Where(x => x.TotalPage == null);
                }
                
                return query;
            }
        }

        [Theory, AutoMoqData(count: 64)]
        public void BuildExpression_TotalPageEqWithAnd_ShouldMatchCount(List<Book> data, int totalPage)
        {
            // Arrange
            var filter = new BookFilter_OperatorFilter_TotalPage
            {
                TotalPage = new OperatorFilter<int>
                {
                    Eq = totalPage, 
                    CombineWith = CombineType.And
                }
            };

            // Act
            var query = data.AsQueryable().ApplyFilter(filter);
            var result = query.ToList();

            // Assert
            var actualResult = data.AsQueryable().Where(x => x.TotalPage == totalPage).ToList();

            Assert.Equal(actualResult.Count, result.Count);
            foreach (var item in actualResult)
                Assert.Contains(item, result);
        }

        [Theory, AutoMoqData(count: 64)]
        public void BuildExpression_TotalPageNotWithAnd_ShouldMatchCount(List<Book> data, int totalPage)
        {
            // Arrange
            var filter = new BookFilter_OperatorFilter_TotalPage
            {
                TotalPage = new OperatorFilter<int>
                {
                    Not = totalPage, 
                    CombineWith = CombineType.And
                }
            };

            // Act
            var query = data.AsQueryable().ApplyFilter(filter);
            var result = query.ToList();

            // Assert
            var actualResult = data.AsQueryable().Where(x => x.TotalPage != totalPage).ToList();

            Assert.Equal(actualResult.Count, result.Count);
            foreach (var item in actualResult)
                Assert.Contains(item, result);
        }

        [Theory, AutoMoqData(count: 64)]
        public void BuildExpression_TotalPageGtWithAnd_ShouldMatchCount(List<Book> data, int totalPage)
        {
            // Arrange
            var filter = new BookFilter_OperatorFilter_TotalPage
            {
                TotalPage = new OperatorFilter<int>
                {
                    Gt = totalPage, 
                    CombineWith = CombineType.And
                }
            };

            // Act
            var query = data.AsQueryable().ApplyFilter(filter);
            var result = query.ToList();

            // Assert
            var actualResult = data.AsQueryable().Where(x => x.TotalPage > totalPage).ToList();

            Assert.Equal(actualResult.Count, result.Count);
            foreach (var item in actualResult)
                Assert.Contains(item, result);
        }

        [Theory, AutoMoqData(count: 64)]
        public void BuildExpression_TotalPageGteWithAnd_ShouldMatchCount(List<Book> data, int totalPage)
        {
            // Arrange
            var filter = new BookFilter_OperatorFilter_TotalPage
            {
                TotalPage = new OperatorFilter<int>
                {
                    Gte = totalPage, 
                    CombineWith = CombineType.And
                }
            };

            // Act
            var query = data.AsQueryable().ApplyFilter(filter);
            var result = query.ToList();

            // Assert
            var actualResult = data.AsQueryable().Where(x => x.TotalPage >= totalPage).ToList();

            Assert.Equal(actualResult.Count, result.Count);
            foreach (var item in actualResult)
                Assert.Contains(item, result);
        }

        [Theory, AutoMoqData(count: 64)]
        public void BuildExpression_TotalPageLteWithAnd_ShouldMatchCount(List<Book> data, int totalPage)
        {
            // Arrange
            var filter = new BookFilter_OperatorFilter_TotalPage
            {
                TotalPage = new OperatorFilter<int>
                {
                    Lte = totalPage, 
                    CombineWith = CombineType.And
                }
            };

            // Act
            var query = data.AsQueryable().ApplyFilter(filter);
            var result = query.ToList();

            // Assert
            var actualResult = data.AsQueryable().Where(x => x.TotalPage <= totalPage).ToList();

            Assert.Equal(actualResult.Count, result.Count);
            foreach (var item in actualResult)
                Assert.Contains(item, result);
        }

        [Theory, AutoMoqData(count: 64)]
        public void BuildExpression_TotalPageLtWithAnd_ShouldMatchCount(List<Book> data, int totalPage)
        {
            // Arrange
            var filter = new BookFilter_OperatorFilter_TotalPage
            {
                TotalPage = new OperatorFilter<int>
                {
                    Lt = totalPage, 
                    CombineWith = CombineType.And
                }
            };

            // Act
            var query = data.AsQueryable().ApplyFilter(filter);
            var result = query.ToList();

            // Assert
            var actualResult = data.AsQueryable().Where(x => x.TotalPage < totalPage).ToList();

            Assert.Equal(actualResult.Count, result.Count);
            foreach (var item in actualResult)
                Assert.Contains(item, result);
        }

        [Theory, AutoMoqData(count: 64)]
        public void BuildExpression_TotalPageLtGtWithAnd_ShouldMatchCount(List<Book> data, int min, int some)
        {
            // Arrange
            var max = min + some;
            var filter = new BookFilter_OperatorFilter_TotalPage
            {
                TotalPage = new OperatorFilter<int>
                {
                    Gt = min,
                    Lt = max, 
                    CombineWith = CombineType.And
                }
            };

            // Act
            var query = data.AsQueryable().ApplyFilter(filter);
            var result = query.ToList();

            // Assert
            var actualResult = data.AsQueryable().Where(x => x.TotalPage > min && x.TotalPage < max).ToList();

            Assert.Equal(actualResult.Count, result.Count);
            foreach (var item in actualResult)
                Assert.Contains(item, result);
        }

        [Theory, AutoMoqData(count: 64)]
        public void BuildExpression_TotalPageLtGtWithOr_ShouldMatchCount(List<Book> data, int min, int some)
        {
            // Arrange
            var max = min + some;
            var filter = new BookFilter_OperatorFilter_TotalPage
            {
                TotalPage = new OperatorFilter<int>
                {
                    Gt = min,
                    Lt = max, 
                    CombineWith = CombineType.Or
                }
            };

            // Act
            var query = data.AsQueryable().ApplyFilter(filter);
            var result = query.ToList();

            // Assert
            var actualResult = data.AsQueryable().Where(x => x.TotalPage > min || x.TotalPage < max).ToList();

            Assert.Equal(actualResult.Count, result.Count);
            foreach (var item in actualResult)
                Assert.Contains(item, result);
        }
        
        [Theory, AutoMoqData(count: 64)]
        public void BuildExpression_TotalPageLtEqWithOr_ShouldMatchCount(List<Book> data, int max, int exact)
        {
            // Arrange
            var filter = new BookFilter_OperatorFilter_TotalPage
            {
                TotalPage = new OperatorFilter<int>
                {
                    Eq = exact,
                    Lt = max, 
                    CombineWith = CombineType.Or
                }
            };

            // Act
            var query = data.AsQueryable().ApplyFilter(filter);
            var result = query.ToList();

            // Assert
            var actualResult = data.AsQueryable().Where(x => x.TotalPage < max || x.TotalPage == exact).ToList();

            Assert.Equal(actualResult.Count, result.Count);
            foreach (var item in actualResult)
                Assert.Contains(item, result);
        }
        
        [Theory, AutoMoqData(count: 64)]
        public void BuildExpression_ViewsIsNull_ShouldMatchCount(List<Book> data)
        {
            for (var i = 0; i < 5; i++)
            {
                data[i].Views = null;
            }
            
            // Arrange
            var filter = new BookFilter_OperatorFilter_Views
            {
                Views = new OperatorFilter<int>
                {
                    IsNull = true
                }
            };

            // Act
            var query = data.AsQueryable().ApplyFilter(filter);
            var result = query.ToList();

            // Assert
            var actualResult = data.AsQueryable().Where(x => x.Views == null).ToList();

            Assert.Equal(actualResult.Count, result.Count);
            Assert.Equal(5, actualResult.Count);
            Assert.Equal(5, result.Count);
            foreach (var item in actualResult)
                Assert.Contains(item, result);
        }
        
        [Theory, AutoMoqData(count: 64)]
        public void BuildExpression_ViewsIsNotNull_ShouldMatchCount(List<Book> data)
        {
            for (var i = 0; i < 5; i++)
            {
                data[i].Views = null;
            }
            
            // Arrange
            var filter = new BookFilter_OperatorFilter_Views
            {
                Views = new OperatorFilter<int>
                {
                    IsNotNull = true
                }
            };

            // Act
            var query = data.AsQueryable().ApplyFilter(filter);
            var result = query.ToList();

            // Assert
            var actualResult = data.AsQueryable().Where(x => x.Views != null).ToList();

            Assert.Equal(actualResult.Count, result.Count);
            Assert.Equal(64 - 5, actualResult.Count);
            Assert.Equal(64 - 5, result.Count);
            foreach (var item in actualResult)
                Assert.Contains(item, result);
        }
    }
}
