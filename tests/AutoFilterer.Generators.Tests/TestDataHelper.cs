using AutoFilterer.Generators.Tests.Environment.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoFilterer.Generators.Tests;

public static class TestDataHelper
{
    public static List<Environment.Models.Book> GetSampleBooks()
    {
        return new List<Environment.Models.Book>
        {
            new Environment.Models.Book { Id = 1, Title = "Clean Code", TotalPage = 464, Year = 2008, IsPublished = true, AuthorId = 1, Author = "Robert C. Martin", ReadCount = 100, Views = 5000 },
            new Environment.Models.Book { Id = 2, Title = "The Pragmatic Programmer", TotalPage = 352, Year = 1999, IsPublished = true, AuthorId = 2, Author = "Andrew Hunt", ReadCount = 200, Views = 3000 },
            new Environment.Models.Book { Id = 3, Title = "Design Patterns", TotalPage = 395, Year = 1994, IsPublished = true, AuthorId = 3, Author = "Eric Evans", ReadCount = 150, Views = 4000 },
            new Environment.Models.Book { Id = 4, Title = "Refactoring", TotalPage = 448, Year = 1999, IsPublished = true, AuthorId = 2, Author = "Andrew Hunt", ReadCount = 180, Views = 4500 },
            new Environment.Models.Book { Id = 5, Title = "Test Driven Development", TotalPage = 240, Year = 2002, IsPublished = true, AuthorId = 1, Author = "Robert C. Martin", ReadCount = 120, Views = 3500 },
            new Environment.Models.Book { Id = 6, Title = "Working Effectively with Legacy Code", TotalPage = 456, Year = 2004, IsPublished = true, AuthorId = 1, Author = "Robert C. Martin", ReadCount = 90, Views = 4200 },
            new Environment.Models.Book { Id = 7, Title = "Domain-Driven Design", TotalPage = 560, Year = 2003, IsPublished = true, AuthorId = 3, Author = "Eric Evans", ReadCount = 110, Views = 5500 },
            new Environment.Models.Book { Id = 8, Title = "Continuous Delivery", TotalPage = 512, Year = 2010, IsPublished = true, AuthorId = 2, Author = "Andrew Hunt", ReadCount = 130, Views = 3800 },
            new Environment.Models.Book { Id = 9, Title = "The Clean Coder", TotalPage = 256, Year = 2011, IsPublished = false, AuthorId = 1, Author = "Robert C. Martin", ReadCount = 80, Views = 2800 },
            new Environment.Models.Book { Id = 10, Title = "Agile Software Development", TotalPage = 552, Year = 2002, IsPublished = true, AuthorId = 1, Author = "Robert C. Martin", ReadCount = 140, Views = 4900 },
        };
    }

    public static List<Environment.Models.Author> GetSampleAuthors()
    {
        var authors = new List<Environment.Models.Author>
        {
            new Environment.Models.Author { Id = 1, Name = "Robert C. Martin", Country = "USA", Age = 70 },
            new Environment.Models.Author { Id = 2, Name = "Andrew Hunt", Country = "USA", Age = 65 },
            new Environment.Models.Author { Id = 3, Name = "Eric Evans", Country = "USA", Age = 55 },
        };

        var books = GetSampleBooks();
        foreach (var author in authors)
        {
            author.Books = books
                .Where(b => b.AuthorId == author.Id)
                .Select(b => new Environment.Models.Book
                {
                    Id = b.Id,
                    Title = b.Title,
                    TotalPage = b.TotalPage,
                    Year = b.Year,
                    IsPublished = b.IsPublished,
                    AuthorId = b.AuthorId,
                    Author = b.Author,
                    ReadCount = b.ReadCount,
                    Views = b.Views,
                })
                .ToList();
            foreach (var book in author.Books)
            {
                book.AuthorModel = author;
            }
        }

        return authors;
    }

    public static List<Environment.Models.Publisher> GetSamplePublishers()
    {
        var authors = GetSampleAuthors();
        
        return new List<Environment.Models.Publisher>
        {
            new Environment.Models.Publisher { Id = 1, Name = "Pearson", Authors = authors.Where(a => a.Id == 1).ToList() },
            new Environment.Models.Publisher { Id = 2, Name = "Addison-Wesley", Authors = authors.Where(a => a.Id == 2 || a.Id == 3).ToList() },
        };
    }

    public static List<Environment.Models.Preferences> GetSamplePreferences()
    {
        return new List<Environment.Models.Preferences>
        {
            new Environment.Models.Preferences 
            { 
                UserId = Guid.Parse("11111111-1111-1111-1111-111111111111"), 
                IsTwoFactorEnabled = true, 
                GivenName = "Alice", 
                SecurityLevel = 1, 
                ReadLimit = 100,
                OrganizationUnitId = Guid.Parse("11111111-1111-1111-1111-111111111111")
            },
            new Environment.Models.Preferences 
            { 
                UserId = Guid.Parse("22222222-2222-2222-2222-222222222222"), 
                IsTwoFactorEnabled = false, 
                GivenName = "Bob", 
                SecurityLevel = 2, 
                ReadLimit = 200,
                OrganizationUnitId = Guid.Parse("33333333-3333-3333-3333-333333333333")
            },
            new Environment.Models.Preferences 
            { 
                UserId = Guid.Parse("33333333-3333-3333-3333-333333333333"), 
                IsTwoFactorEnabled = true, 
                GivenName = "Charlie", 
                SecurityLevel = 3, 
                ReadLimit = 300,
                OrganizationUnitId = Guid.Parse("11111111-1111-1111-1111-111111111111")
            },
            new Environment.Models.Preferences 
            { 
                UserId = Guid.Parse("44444444-4444-4444-4444-444444444444"), 
                IsTwoFactorEnabled = false, 
                GivenName = "Diana", 
                SecurityLevel = 1, 
                ReadLimit = null,
                OrganizationUnitId = Guid.Parse("22222222-2222-2222-2222-222222222222")
            },
        };
    }
}
