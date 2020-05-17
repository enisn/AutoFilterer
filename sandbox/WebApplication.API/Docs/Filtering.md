# Filtering
AutoFilterer aims to keep strong type models and just removes complicated algorithms.

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
public class BookFilter : FilterBase // <-- Just inherit FilterBase
{
    public string Title { get; set; }
    public string Author { get; set; }
    public int? Year { get; set; } // <-- ValueTypes must be nullable
    // ...
    // Only written properties can be filterable
}
```

