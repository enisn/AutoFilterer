# Combine Parameters (Or/And)
You can decide combining filtering parameters with **OR** (`||`) / **AND** (`&&`)

By default your parameters will be combined with `&&` and if you're using swagger, you'll see `CombineWith` parameter. This is enum and includes `And` and `Or` values. Default is `0`(And), you can send that parameter as `1` and you'll se your multiple parameters will be combined with `||`.

- Let say you have a model something like that again:

```csharp
public class Blog
{
    public string BlogId { get; set; }
    public int CategoryId { get; set; }
    public int Priority { get; set; }
    public bool IsPublished { get; set; }
    public DateTime PublishDate { get; set; }
}
```

- Let's create a filtering DTO like that:

```csharp
public class BlogFilterDto : FilterBase<Blog>
{
    public int CategoryId { get; set; }
    public Range<int> Priority { get; set; }
    public bool? IsPublished { get; set; }
}
```

- Let's create a sample Controller and get the DTO from querystring

```csharp
public class BlogsController : ControllerBase
{
    [HttpGet]
    public IActionResult Get([FromQuery]BlogFilterDto filter)
    {
        using(var db = new MyDbContext())
        {
            // In this case, your expression will be built like that:
            // QueryString: /Blogs?priority.min=4&isPublished=false
            // x => x.Priority > 4 && x.IsPublished == false
            var blogList = db.Blogs.ApplyFilter(filter).ToList();
            return Ok(blogList);
        }
    }
}
```

## Case 1 (In-Code)
- You can define combining from code

```csharp
public class BlogsController : ControllerBase
{
    [HttpGet]
    public IActionResult Get([FromQuery]BlogFilterDto filter)
    {
        using(var db = new MyDbContext())
        {
            filter.CombineWith = CombineType.Or; // <-- Can be defined here

            // In this case, your expression will be built like that:
            // QueryString: /Blogs?priority.min=4&isPublished=false
            // x => x.Priority > 4 || x.IsPublished == false
            var blogList = db.Blogs.ApplyFilter(filter).ToList();
            return Ok(blogList);
        }
    }
}
```

## Case 2 (From-Query)
- You can define combining from query string

```csharp
public class BlogsController : ControllerBase
{
    [HttpGet]
    public IActionResult Get([FromQuery]BlogFilterDto filter)
    {
        using(var db = new MyDbContext())
        {
            // In this case, your expression will be built like that:
            // QueryString: /Blogs?priority.min=4&isPublished=false&combineWith=1
            // x => x.Priority > 4 || x.IsPublished == false
            var blogList = db.Blogs.ApplyFilter(filter).ToList();
            return Ok(blogList);
        }
    }
}
```
