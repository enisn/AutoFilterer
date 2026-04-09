using System;
using System.Collections.Generic;

namespace AutoFilterer.Generators.Tests.Environment.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public int TotalPage { get; set; }
    public int ReadCount { get; set; }
    public int Year { get; set; }
    public bool IsPublished { get; set; }
    public int? Views { get; set; }
    
    // Navigation property
    public int AuthorId { get; set; }
    public Author AuthorModel { get; set; }
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

public class Preferences
{
    public Guid UserId { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
    public string GivenName { get; set; }
    public int SecurityLevel { get; set; }
    public int? ReadLimit { get; set; }
    public Guid? OrganizationUnitId { get; set; }
}

// Recursive hierarchical models for Phase 2 N-depth mixed quantifier tests
public class Level8
{
    public int Id { get; set; }
    public string Value { get; set; }
    public int Score { get; set; }
}

public class Level7
{
    public int Id { get; set; }
    public string Value { get; set; }
    public List<Level8> Children { get; set; } = new();
}

public class Level6
{
    public int Id { get; set; }
    public string Value { get; set; }
    public List<Level7> Children { get; set; } = new();
}

public class Level5
{
    public int Id { get; set; }
    public string Value { get; set; }
    public List<Level6> Children { get; set; } = new();
}

public class Level4
{
    public int Id { get; set; }
    public string Value { get; set; }
    public List<Level5> Children { get; set; } = new();
}

public class Level3
{
    public int Id { get; set; }
    public string Value { get; set; }
    public List<Level4> Children { get; set; } = new();
}

public class Level2
{
    public int Id { get; set; }
    public string Value { get; set; }
    public List<Level3> Children { get; set; } = new();
}

public class Level1
{
    public int Id { get; set; }
    public string Value { get; set; }
    public List<Level2> Children { get; set; } = new();
}
