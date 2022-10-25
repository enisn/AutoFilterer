# Basics

Entity:
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

Create a filter object for Blog Entity:
```csharp
public class BlogFilterDto : FilterBase
{
    public int CategoryId { get; set; }
    public int Priority { get; set; }
    public bool? IsPublished { get; set; }
}
```

- Create a sample Controller and get the DTO from querystring:

```csharp
public class BlogsController : ControllerBase
{
    [HttpGet]
    public IActionResult Get([FromQuery]BlogFilterDto filter)
    {
        using(var db = new MyDbContext())
        {
            var blogList = db.Blogs.ApplyFilter(filter).ToList();
            return Ok(blogList);
        }
    }
}
```

- Just send following requests to check result:

  * `/Blogs?IsPublished=False`
  * `/Blogs?CategoryId=4`
  * `/Blogs?Priority=4`
  * `/Blogs?IsPublished=True&Priority=1`
  * `/Blogs?IsPublished=True&Priority=5&CategoryId=1`

***
