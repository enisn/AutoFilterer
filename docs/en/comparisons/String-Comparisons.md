# Using string comparisons

- Let's add a string column to model:


```csharp
public class Blog
{
    public string BlogId { get; set; }
    public int CategoryId { get; set; }
    public string Title { get; set; } // <-- We'll work on this string field
    public int Priority { get; set; }
    public bool IsPublished { get; set; }
    public DateTime PublishDate { get; set; }
}
```

- And of course add DTO too

```csharp
public class BlogFilterDto : FilterBase<Blog>
{
    public int? CategoryId { get; set; }
    public int? Priority { get; set; }
    public string Title { get; set; } // <-- Same property name with Entity's property
    public bool? IsPublished { get; set; }
}
```

- Let's add `StringFilterOptions` attribute on the string field to search as **Contains** instead of exact value:

```csharp
public class BlogFilterDto : FilterBase<Blog>
{
    public int? CategoryId { get; set; }
    public int? Priority { get; set; }
    [StringFilterOptions(StringFilterOption.Contains)]
    public string Title { get; set; }
    public bool? IsPublished { get; set; }
}
```

- You can send following requests to check result. That's awesome!

  * `/Blogs?Title=Hello`
  * `/Blogs?Title=Hello%20World`
  * `/Blogs?Title=a`

***