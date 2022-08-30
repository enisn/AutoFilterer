# Filtering
AutoFilterer aim is keeping models strong-typed and removing complicated algorithms while filtering. With AutoFilterer, you define only filter object with rules and  do not write any LINQ Expression, AutoFilterer does it for you.

## Implementation
Create a filter object that inherits from `FilterBase`. Create a property for each field you want to filter.


- Property names should match entity property names. If not use `[CompareTo]` attribute.

- **ValueTypes** should be **nullable** in case the value is not sent. Otherwise, even the value isn't sent, the filter will be applied with the default value of ValueType.


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

> _(If property names doesn't match or can't match, you can use `[CompareTo]` attribute to define entity property name. Check out [Property name mapping](examples/Property-Name-Mapping.md) for more info)_.

## Usage

```csharp
[HttpGet]
public IActionResult Get([FromQuery]BookFilter filter)
{
    var result = db.Books.ApplyFilter(filter).ToList();
    return Ok(result);
}
```

Those properties can be used in the following ways in querystring.

`/books?title=Middlemarch`
`/books?title=Nostromo&year=1904`

## Further reading

- [String Comparisons](comparisons/String-Comparisons.md)
- [Operator Comparisons](comparisons/Operator-Comparisons.md)