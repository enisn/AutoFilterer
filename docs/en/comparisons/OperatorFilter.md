### OperatorFilter
This filter includes some simple comparisons: 
`eq`, `not`, `gt`, `gte`, `lt`, `lte`

#### Implementation
Place **OperatorFilter<T>** to your Filter model:

```csharp
public class BookFilter : FilterBase
{
    public OperatorFilter<int> TotalPage { get; set; }
}
```

Use **ApplyFilter()** method as always:

```csharp
public IList<Book> GetBooks(BookFilter filter)
{
    return db.Books.ApplyFilter(filter).ToList();
}
```

#### Usage
- You can launch swagger document to see all arguments.

- Just try sample request like below to get books which has 300 pages at least.
`/books?totalPage.gte=300`

- And handler controller as always:

```csharp
[HttpGet]
public IActionResult Get([FromQuery]BookFilter filter)
{
    var result = db.Books.ApplyFilter(filter).ToList();
    return Ok();
}
```

## What is next?
You may also see:
- [StringFilter](StringFilter)