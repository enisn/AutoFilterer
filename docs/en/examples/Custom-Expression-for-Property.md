# Custom Expression for a Property
Sometimes one of your property can be used for filtering multiple properties of something special. So you can create your own attributes to make custom expression for your custom queries. Following entity models will be used in this document:

- Blog.cs
```csharp
pulic class Blog
{
    public string BlogId { get; set; }
    public int CategoryId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public bool IsPublished { get; set; }
    public virtual ICollection<Comment> Comments { get; set; }
    public virtual Author Author { get; set; } 
}
```

---

# Attributes
You can create your own attributes for your custom expression requirements. Create a class which inherits `FilteringOptionsBaseAttribute` class and override abstract methods. Let's make a sample. 
For example our `Search` property will search in Blog Title and Content.

- Create your filtering dto:
```csharp
public class SearchDto : FilterBase
{
    [FromQuery(Name = "q")]
    public string Search { get; set; }
}
```

- Then Create an attribute something like that:
```csharp
 public class BlogSearchAttribute : FilteringOptionsBaseAttribute
    {
        public override Expression BuildExpression(Expression expressionBody, PropertyInfo targetProperty, PropertyInfo filterProperty, object value)
        {
            // Also means:  x => x.Title == "someValue" || x.Content == "someValue"
            return Expression.Or(
                    Expression.Equal(
                        Expression.Property(expressionBody, "Title"),
                        Expression.Constant(value)
                        ),
                    Expression.Equal(
                        Expression.Property(expressionBody, "Content"),
                        Expression.Constant(value)
                        )
                );
        }
    }
```

- Then go and place your attribute over your property:
```csharp
public class SearchDto : FilterBase
{
    [FromQuery(Name = "q")]
    [BlogSearchAttribute] // <-- Place here
    public string Search { get; set; }
}
```

---

# Types
You can create your own type with own expression generation like [Range<T>](/enisn/AutoFilterer/blob/master/src/AutoFilterer/Types/Range.cs) object. Just create your class, inherit and implement `IFilterableType` interface. Autfilterer will use your implemented `BuildExpression()` method.
