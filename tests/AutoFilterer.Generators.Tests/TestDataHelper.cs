using AutoFilterer.Generators.Tests.Environment.Models;
using System.Collections.Generic;
using System.Linq;

namespace AutoFilterer.Generators.Tests;

public static class TestDataHelper
{
    public static List<Environment.Models.Book> GetSampleBooks()
    {
        return new List<Environment.Models.Book>
        {
            new Environment.Models.Book { Id = 1, Title = "Clean Code", TotalPage = 464, Year = 2008, IsPublished = true, AuthorId = 1 },
            new Environment.Models.Book { Id = 2, Title = "The Pragmatic Programmer", TotalPage = 352, Year = 1999, IsPublished = true, AuthorId = 2 },
            new Environment.Models.Book { Id = 3, Title = "Design Patterns", TotalPage = 395, Year = 1994, IsPublished = true, AuthorId = 3 },
            new Environment.Models.Book { Id = 4, Title = "Refactoring", TotalPage = 448, Year = 1999, IsPublished = true, AuthorId = 2 },
            new Environment.Models.Book { Id = 5, Title = "Test Driven Development", TotalPage = 240, Year = 2002, IsPublished = true, AuthorId = 1 },
            new Environment.Models.Book { Id = 6, Title = "Working Effectively with Legacy Code", TotalPage = 456, Year = 2004, IsPublished = true, AuthorId = 1 },
            new Environment.Models.Book { Id = 7, Title = "Domain-Driven Design", TotalPage = 560, Year = 2003, IsPublished = true, AuthorId = 3 },
            new Environment.Models.Book { Id = 8, Title = "Continuous Delivery", TotalPage = 512, Year = 2010, IsPublished = true, AuthorId = 2 },
            new Environment.Models.Book { Id = 9, Title = "The Clean Coder", TotalPage = 256, Year = 2011, IsPublished = false, AuthorId = 1 },
            new Environment.Models.Book { Id = 10, Title = "Agile Software Development", TotalPage = 552, Year = 2002, IsPublished = true, AuthorId = 1 },
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
            author.Books = books.Where(b => b.AuthorId == author.Id).ToList();
            foreach (var book in author.Books)
            {
                book.Author = author;
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
}
