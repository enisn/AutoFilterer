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




*TODO: Move following topics here:*

[Filtering · enisn/AutoFilterer Wiki (github.com)](https://github.com/enisn/AutoFilterer/wiki/Filtering)

[Array Search · enisn/AutoFilterer Wiki (github.com)](https://github.com/enisn/AutoFilterer/wiki/Array-Search)

[Advanced Queries · enisn/AutoFilterer Wiki (github.com)](https://github.com/enisn/AutoFilterer/wiki/Advanced-Queries)

[Operator Comparisons · enisn/AutoFilterer Wiki (github.com)](https://github.com/enisn/AutoFilterer/wiki/Operator-Comparisons)

[String Comparisons · enisn/AutoFilterer Wiki (github.com)](https://github.com/enisn/AutoFilterer/wiki/String-Comparisons)

[OperatorComparison · enisn/AutoFilterer Wiki (github.com)](https://github.com/enisn/AutoFilterer/wiki/OperatorComparison)

[OperatorFilter · enisn/AutoFilterer Wiki (github.com)](https://github.com/enisn/AutoFilterer/wiki/OperatorFilter)

[StringFilter · enisn/AutoFilterer Wiki (github.com)](https://github.com/enisn/AutoFilterer/wiki/StringFilter)

[Working with Range · enisn/AutoFilterer Wiki (github.com)](https://github.com/enisn/AutoFilterer/wiki/Working-with-Range)



