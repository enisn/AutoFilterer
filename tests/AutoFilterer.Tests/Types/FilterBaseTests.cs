using AutoFilterer.Extensions;
using AutoFilterer.Tests.Core;
#if LEGACY_NAMESPACE
using AutoFilterer.Enums;
#endif
using AutoFilterer.Tests.Environment.Dtos;
using AutoFilterer.Tests.Environment.Models;
using AutoFilterer.Types;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AutoFilterer.Tests.Types;

public class FilterBaseTests : IDisposable
{
    private MockRepository mockRepository;
    private static readonly Random random = new Random();

    private readonly List<User> dummyData = new List<User>
        {
            new User
            {
                Id = Guid.NewGuid(),
                Email = "abc@amail.com",
                FullName="John Doe",
                IsActive = true,
                Preferences = new Preferences
                {
                    GivenName = "jd1990",
                    IsTwoFactorEnabled = true,
                    SecurityLevel = 5
                },
                Books = new List<Book>
                {
                    new Book{ ReadCount = 50, Title = "Normal People", Author = "Sally Rooney", TotalPage = 300, IsPublished = true },
                    new Book{ ReadCount = 12, Title = "Educated", Author = "Tara Westover", TotalPage = 190, IsPublished = false },
                    new Book{ ReadCount = 367, Title = "Sapiens", Author = "Yuval Noah Harari", TotalPage = 465, IsPublished = false },
                }
            },
            new User
            {
                Id = Guid.NewGuid(),
                Email = "def@bmail.com",
                FullName="Michael D.",
                IsActive = true,
                Preferences = new Preferences
                {
                    GivenName = "md2000",
                    IsTwoFactorEnabled = true,
                    SecurityLevel = 1
                },
                Books = new List<Book>
                {
                    new Book{ ReadCount = 50, Title = "The Testaments", Author = "Margaret Atwood", TotalPage = 230, IsPublished = false },
                    new Book{ ReadCount = 12, Title = "Ducks, Newburyport", Author = "Lucy Ellmann", TotalPage = 270, IsPublished = true },
                    new Book{ ReadCount = 367, Title = "Girl, Woman, Other", Author = "Bernardine Evaristo", TotalPage = 265, IsPublished = true },
                }
            },
            new User
            {
                Id = Guid.NewGuid(),
                Email = "ghi@bmail.com",
                FullName="Alice Swinson",
                IsActive = false,
                Preferences = new Preferences
                {
                    GivenName = "alice.1994",
                    IsTwoFactorEnabled = false,
                    SecurityLevel = 5
                },
                Books = new List<Book>
                {
                    new Book { ReadCount = 70, Title = "Normal People", Author = "Sally Rooney", TotalPage = 230, IsPublished = false },
                }
            }
        };


    public FilterBaseTests()
    {
        this.mockRepository = new MockRepository(MockBehavior.Strict);


    }

    public void Dispose()
    {
        this.mockRepository.VerifyAll();
    }

    private FilterBase CreateFilterBase()
    {
        return new FilterBase();
    }

    [Theory, AutoMoqData]
    public void ApplyFilterTo_WithEmptyFilterBase_ShouldMatchExpected(List<Book> data)
    {
        // Arrange
        var filter = new BookFilterBase();

        // Act
        var query = data.AsQueryable().ApplyFilter(filter);
        var result = query.ToList();

        // Assert
        var expected = data.AsQueryable().ToList();

        Assert.Equal(expected.Count, result.Count);
        foreach (var item in expected)
            Assert.Contains(item, result);
    }

    [Theory, AutoMoqData]
    public void ApplyFilterTo_WithEmptyPaginationFilterBase_ShouldMatchExpected(List<Book> data)
    {
        // Arrange
        var filter = new BookFilter_Orderable();

        // Act
        var query = data.AsQueryable().ApplyFilter(filter);
        var result = query.ToList();

        // Assert
        var expected = data.AsQueryable().ToList();

        Assert.Equal(expected.Count, result.Count);
        foreach (var item in expected)
            Assert.Contains(item, result);
    }

    [Theory, AutoMoqData]
    public void ApplyFilterTo_WithSingleField_ShouldMatchCount(List<User> dummyData)
    {
        // Arrange
        var filterBase = new UserFilterBase
        {
            Email = dummyData.FirstOrDefault().Email,
        };

        IQueryable<User> query = dummyData.AsQueryable();

        // Act
        var result = query.ApplyFilter(filterBase).ToList();

        // Assert
        Assert.True(result.Count == dummyData.Count(x => x.Email == filterBase.Email));
    }

    [Theory, AutoMoqData]
    public void ApplyFilterTo_WithTwoField_ShouldMatchCount(List<User> dummyData)
    {
        // Arrange
        var filterBase = new UserFilterBase
        {
            Email = dummyData.FirstOrDefault().Email,
            IsActive = dummyData.FirstOrDefault().IsActive,
        };

        IQueryable<User> query = dummyData.AsQueryable();

        // Act
        filterBase.CombineWith = CombineType.And;
        var result = query.ApplyFilter(filterBase).ToList();
        filterBase.CombineWith = CombineType.Or;
        var orResult = query.ApplyFilter(filterBase).ToList();


        // Assert
        Assert.True(result.Count == dummyData.Count(x => x.Email == filterBase.Email && x.IsActive == filterBase.IsActive));
        Assert.True(orResult.Count == dummyData.Count(x => x.Email == filterBase.Email || x.IsActive == filterBase.IsActive));
    }

    [Theory, AutoMoqData]
    public void BuildExpression_WithContainsString_ShouldMatchCount(List<User> dummyData)
    {
        // Arrange
        var partOfName = dummyData.FirstOrDefault()?.FullName?.Substring(0, 4);
        var filterBase = new UserFilterBase
        {
            FullName = partOfName,
        };

        var query = dummyData.AsQueryable();

        // Act
        var result = query.ApplyFilter(filterBase).ToList();

        // Assert
        Assert.True(result.Count == dummyData.Count(x => x.FullName.Contains(filterBase.FullName, StringComparison.InvariantCultureIgnoreCase)));
    }

    [Theory, AutoMoqData(15)]
    public void BuildExpression_WithSingleBooleanParameter_ShouldMatchCount(bool? isActive, List<User> dummyData)
    {
        // Arrange
        var filterBase = new UserFilterBase
        {
            IsActive = isActive
        };

        var query = dummyData.AsQueryable();

        // Act
        var result = query.ApplyFilter(filterBase).ToList();

        // Assert

        Func<User, bool> condition = _ => true;
        if (isActive != null)
            condition = x => x.IsActive == isActive;

        Assert.True(result.Count == dummyData.Count(condition));
    }


    [Theory]
    [InlineData("1994")]
    [InlineData("90")]
    [InlineData("0")]
    public void BuildExpression_InnerSingleObjectWithSingleParameter_ShouldMatchCount(string givenName)
    {
        // Arrange

        var filterBase = new UserFilterBase
        {
            Preferences = new PreferencesFilterBase { GivenName = givenName }
        };

        var query = dummyData.AsQueryable();

        // Act
        var filteredQuery = query.ApplyFilter(filterBase);
        var result = filteredQuery.ToList();

        // Assert
        var actualResult = dummyData.Where(x => x.Preferences.GivenName.EndsWith(filterBase.Preferences.GivenName)).ToList();

        Assert.True(result.Count == actualResult.Count);
        foreach (var item in actualResult)
            Assert.Contains(item, actualResult);
    }

    [Fact]
    public void BuildExpression_InnerSingleObjectWithCollectionFilter_ShouldMatchCount()
    {
        // Arrange

        var filter = new UserFilterBase
        {
            Books = new BookFilterBase { Title = "Normal" }
        };

        var query = dummyData.AsQueryable();

        // Act
        var filteredQuery = query.ApplyFilter(filter);
        Console.WriteLine(filteredQuery);
        var result = filteredQuery.ToList();

        // Assert
        var actualResult = dummyData.Where(x => x.Books.Any(a => a.Title.Contains(filter.Books.Title, StringComparison.InvariantCultureIgnoreCase))).ToList();

        Assert.True(result.Count == actualResult.Count);
        foreach (var item in actualResult)
            Assert.Contains(item, actualResult);
    }

    [Theory, AutoMoqData(count: 64)]
    public void BuildExpression_IntoNullableProperty_ShouldMatchCount(List<Preferences> dummyData)
    {
        // Arrange
        var filter = new PreferencesFilter_NullableProperty
        {
            ReadLimit = dummyData.FirstOrDefault(x => random.Next(0, 100) < 50)?.ReadLimit
        };

        var query = dummyData.AsQueryable();

        // Act
        var actualQuery = query.ApplyFilter(filter);
        var expectedQuery = query.Where(x => x.ReadLimit.Value == filter.ReadLimit);

        // Assert
        Assert.Equal(expectedQuery.Count(), actualQuery.Count());
    }
}
