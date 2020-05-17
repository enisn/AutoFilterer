# Working With Range

You may want to search a range like between two DateTimes or numbers.

- The Model is same as previous sample:

```csharp
public class Blog
{
    public string BlogId { get; set; }
    public int CategoryId { get; set; }
    public string Title { get; set; }
    public int Priority { get; set; }
    public bool IsPublished { get; set; }
    public DateTime PublishDate { get; set; }
}
```

- Just use following `Range<T>` type in your DTO:

**WARNING:** Do not use nullable types as generic type parameter in `Range<T>`. Use same type with your entity. If your property is already nullable in entity, it's ok. Just use same type with model with Range

```csharp
public class BlogFilterDto : FilterBase<Blog>
{
    public int? CategoryId { get; set; }
    public Range<int> Priority { get; set; } // <-- Careful! Do not use nullable Types as Generic Type parameter
    public string Title { get; set; }
    public bool? IsPublished { get; set; }

    public Range<DateTime> PublishDate { get; set; } // <-- Use Range<DateTime> instead of below
    //public DateTime PublishDate { get; set; }
}
```

- Now, querystring is changed a little bit. Try following requests:


  * `/Blogs?Priority.Min=4`
  * `/Blogs?Priority.Min=3&Priority.Max=5`
  * `/Blogs?PublishDate.Max=01.05.2019` 
    _// Depends on CultureInfo. If you're using Request Localization, each client must send by its own datetime format_

***