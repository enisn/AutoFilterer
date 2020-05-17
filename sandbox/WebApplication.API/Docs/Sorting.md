# Sorting
Sorting is easy to implement. You filter object just should inherit from **OrderableFilterBase**.
After you inherit, OrderableFilterBase applies `query` and `sorting` when you call **ApplyFilter** method.

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
public class BookFilter : OrderableFilterBase // <-- Just inherit OrderableFilterBase 
{
    public string Title { get; set; }
    public string Author { get; set; }
    public int? Year { get; set; }
    // ...
}
```

# Usage
Now your querystring has 2 more parameter. `sort` and `sortBy`. 

- `/books?sort=Title`
- `/books?sort=Title&sortBy=Ascending`
- `/books?sort=Title&sortBy=Descending`
