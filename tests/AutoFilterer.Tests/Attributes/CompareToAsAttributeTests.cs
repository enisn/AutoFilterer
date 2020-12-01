using AutoFilterer.Extensions;
using AutoFilterer.Tests.Environment.Dtos;
using AutoFilterer.Tests.Environment.Models;
using AutoFilterer.Tests.Environment.Statics;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AutoFilterer.Tests.Attributes
{
    public class CompareToAsAttributeTests
    {
        [Theory, AutoMoqData(64)]
        public void BuildExpression_MultipleFieldWithToLowerContainsAttribute_ShouldWorkProperly(List<Book> dummyData)
        {
            // Arrange
            var filter = new BookFilter_CompareToAs_ToLowerContains
            {
                Query = "a",
                CombineWith = CombineType.Or
            };

            var query = dummyData.AsQueryable();
            var expectedQuery = query.Where(x => 
                                    x.Title.ToLower().Contains("a".ToLower()) == true
                                    ||
                                    x.Author.ToLower().Contains("a".ToLower()) == true
                                    );
            var expectedResult = expectedQuery.ToList();

            // Act
            var actualQuery = query.ApplyFilter(filter);
            var actualResult = actualQuery.ToList();

            // Assert
            Assert.Equal(expectedQuery.ToString(), actualQuery.ToString());
            Assert.Equal(expectedResult, actualResult);
        }
    }
}
