using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebApplication.API.Models;

namespace WebApplication.API.Repository;

public class BooksRepository
{
    public BooksRepository()
    {
        var json = File.ReadAllText("Data/default.json");
        this.Books = JsonSerializer.Deserialize<Book[]>(json).AsQueryable();
    }

    public IQueryable<Book> Books { get; }
}
