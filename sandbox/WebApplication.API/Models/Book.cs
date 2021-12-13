using System;

namespace WebApplication.API.Models;

// Generate in root namespace
[GenerateAutoFilter("WebApplication")]
public class Book
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Title { get; set; }
    public string Language { get; set; }
    public string Country { get; set; }
    public string Author { get; set; }
    public int TotalPage { get; set; }
    public int Year { get; set; }
    public string Link { get; set; }
}
