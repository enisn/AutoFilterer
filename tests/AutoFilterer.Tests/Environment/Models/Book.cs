using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilterer.Tests.Environment.Models;

public class Book
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public int TotalPage { get; set; }
    public int ReadCount { get; set; }
    public bool IsPublished { get; set; }
    public int? Views { get; set; }

    public override string ToString()
    {
        return $"[{Id}] {Title} - {Author} | TotalPage: {TotalPage} | ReadCount: {ReadCount}";
    }
}
