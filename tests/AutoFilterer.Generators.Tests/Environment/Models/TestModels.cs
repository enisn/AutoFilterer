using System.Collections.Generic;

namespace AutoFilterer.Generators.Tests.Environment.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int TotalPage { get; set; }
    public int Year { get; set; }
    public bool IsPublished { get; set; }
    
    // Navigation property
    public int AuthorId { get; set; }
    public Author Author { get; set; }
}

public class Author
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Country { get; set; }
    public int? Age { get; set; }
    public List<Book> Books { get; set; } = new();
}

public class Publisher
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Author> Authors { get; set; } = new();
}
