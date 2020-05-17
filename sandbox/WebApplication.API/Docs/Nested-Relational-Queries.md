# Summary
Autofilter supports related entities querying at version `1.1.0` and above. You can filter your One-to-Many, Many-To-Many or One-To-One related entities with nested **FilterBase** objects.

Let's make a sample.


# Over Collections

- Entity class anatomy:
```csharp
pulic class Blog
{
    public string BlogId { get; set; }
    public int CategoryId { get; set; }
    public string Title { get; set; } 
    public virtual ICollection<Comment> Comments { get; set; } // We'll work on this field
}
pulic class Comment
{
    public string CommentId { get; set; }
    public string BlogId { get; set; }
    public string Language { get; set; }
    public string Message { get; set; } 
    public virtual Blog Blog { get; set; }
}
```

- Let's create a dto to filter by `Language` property.

```csharp

pulic class BlogFilterDto : FilterBase
{
    public int? CategoryId { get; set; }
    public string Title { get; set; } 
    public CommentFilterDto Comments { get; set; } // Be careful, it's not a collection and same name with entity.
}
pulic class CommentFilterDto : FilterBase
{
    public string Language { get; set; }
    [StringFilterOptions(StringFilterOption.Contains)]
    public string Message { get; set; }
}
```

- Just apply filter to Blogs:
```csharp
pulic ActionResult Index([FromQuery]BlogFilterDto filter)
{
	using(var db = new MyDbContext())
	{
	    var data = db.Blogs.ApplyFilter(filter).Tolist();
	    return View(data)
	}
}
```
- You can send following requests to check result. No any include required for filtering! That's awesome!

| QueryString | Generated LINQ |
| --- | --- |
|`?Comments.Language=en` | `db.Blogs.Where(x => x.Comments.Any(a => a.Language == "en"))` |
| `?comments.language=en&message=awesome` | `db.Blogs.Where(x => x.Message.Contains("awesome") && x.Comments.Any(a => a.Language == "en"))`


# Over Objects
- Entity class anatomy:
```csharp
pulic class Blog
{
    public string BlogId { get; set; }
    public int CategoryId { get; set; }
    public string Title { get; set; } 
    public virtual Author Author { get; set; } // We'll work on this field
}
pulic class Author
{
    public string AuthorId{ get; set; }
    public string BlogId { get; set; }
    public DateTime RegisterDate { get; set; }
    public string FullName{ get; set; }
    public bool IsPremium { get; set; }
    public virtual Blog Blog { get; set; }
}
```

- Let's create a dto to filter by `Language` property.

```csharp
pulic class BlogFilterDto : FilterBase
{
    public int? CategoryId { get; set; }
    public string Title { get; set; } 
    public AuthorFilterDto Comments { get; set; } // Be careful, it's not a collection and same name with entity.
}
pulic class AuthorFilterDto : FilterBase
{
    public bool? IsPremium { get; set; }
    [StringFilterOptions(StringFilterOption.Contains)]
    public string FullName { get; set; }
}
```

- Then you can send following requests to check result. No any include required for filtering! That's awesome!

| QueryString | Generated LINQ |
| --- | --- |
|`?author.isPremium=True` | `db.Blogs.Where(x => x.Author.IsPremium == true` |
|`?author.isPremium=False&author.fullName=John` | `db.Blogs.Where(x => x.Author.IsPremium == false && x.Author.FullName.Contains("John")`
