# Filtering
AutoFilterer aim is keeping models strong-typed and removing complicated algorithms while filtering. With AutoFilterer, you define only filter object with rules and  do not write any LINQ Expression, AutoFilterer does it for you.

## Implementation
Create a filter object that inherits from `FilterBase`. Create a property for each field you want to filter.


- Property names should match entity property names. If not use `[CompareTo]` attribute.

- **ValueTypes** should be **nullable** in case the value is not sent. Otherwise, even the value isn't sent, the filter will be applied with the default value of ValueType.


```csharp
public class BookFilter : FilterBase // 👈 Inherit from FilterBase
{
    public string Title { get; set; }
    public int? Year { get; set; } // 👈 Value Types have to be nullable
    // ...
    // Only written properties can be filterable
}
```

> _(If property names doesn't match or can't match, you can use `[CompareTo]` attribute to define entity property name. Check out [Property name mapping](examples/Property-Name-Mapping.md) for more info)_.

## Usage

`FilterBase` implements `IFilter` interface that has `ApplyFilter` method. That means, the class you inherit from `FilterBase` is able to generate filters on collections.


```csharp
public IActionResult GetBooks([FromQuery]BookFilter filter)
{
    var query = filter.ApplyFilterTo(db.Books);

    return Ok(query.ToList());
}
```

### ApplyFilter() Extension method

You can also use `ApplyFilter` extension method for `IQueryable<T>` types.


```csharp
public IActionResult GetBooks([FromQuery]BookFilter filter)
{
    return Ok(db.Books.ApplyFilter(filter).ToList());
}
```
Those properties can be used in the following ways in querystring.

`/books?title=Middlemarch`
`/books?title=Nostromo&year=1904`

## Further reading

- [String Comparisons](comparisons/String-Comparisons.md)
- [Operator Comparisons](comparisons/Operator-Comparisons.md)