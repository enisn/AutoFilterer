# Pagination
Sorting is easy to implement. You filter object just should inherit from **PaginationFilterBase**.
After you inherit, PaginationFilterBase applies `query`, `sorting` and `pagination` when you call **ApplyFilter** method.

# Showcase

```csharp
[HttpGet]
public IActionResult Get([FromQuery]BookFilter filter)
{
    var result = db.Books.ApplyFilter(filter).ToList();
    return Ok(result);
}
```

# Implementation
You just need to create a Filter object that includes properties which is able to filter.

```csharp
public class BookFilter : PaginationFilterBase // <-- Just inherit PaginationFilterBase
{
    public string Title { get; set; }
    public string Author { get; set; }
    public int? Year { get; set; }
    // ...
}
```

# Usage
Now your querystring has 2 more parameter. `page` and `perPage`. 

- `/books?page=2`
- `/books?perPage=16`
- `/books?page=2&perPage=16`
