## StringFilter
This is a simple string filtering option. All options are below:
`eq`, `not`, `equals`, `contains`, `startsWith`, `endsWith`

#### Implementation
Place **StringFilter** to your Filter model:

```csharp
public class BookFilter : FilterBase
{
    public StringFilter Title { get; set; }
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

- Just try sample request like below to get books which's title contains thief keyword.
`/books?title.contains=thief`

- And handler controller as always:

```csharp
[HttpGet]
public IActionResult Get([FromQuery]BookFilter filter)
{
    var result = db.Books.ApplyFilter(filter).ToList();
    return Ok();
}
```

***
## What is next?
You may also see:
- [OperatorFilter](OperatorFilter)