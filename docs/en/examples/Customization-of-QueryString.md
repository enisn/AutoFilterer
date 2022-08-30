## Customizing QueryString

You may want to use shorter parameter names in querystring. AspNetCore provides to define querystring parameter's names with `FromQuery` attribute:


```csharp
public class BlogFilterDto : FilterBase<Blog>
{
    [FromQuery(Name="category")]   // < -- This attribute may be used to customize your querystring
    public int CategoryId { get; set; }// <--  But property name must be same with entity
    public int Priority { get; set; }
    public bool? IsPublished { get; set; }
}
```

This provides to handle `category` parameter as CategoryId:

    * `/Blogs?category=4`