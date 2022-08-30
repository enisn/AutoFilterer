## Customizing Pagination Parameters
You may want to use different names instead of `page` and `perPage`

- Just go your Filter Dto and override them:

```csharp
public class BlogFilterDto : PaginationFilterBase<Blog>
{
    public int? CategoryId { get; set; }
    public Range<int> Priority { get; set; }
    public string Title { get; set; }
    public bool? IsPublished { get; set; }
    public Range<DateTime> PublishDate { get; set; }

    [FromQuery(Name = "p")] // <-- you can set querystring name
    public override int Page { get => base.Page; set => base.Page = value; }

    [FromQuery(Name = "limit")]
    public override int PerPage { get => base.PerPage; set => base.PerPage = value; }
}
```

- You can change default values also:
```csharp
    public class BlogFilterDto : PaginationFilterBase<Blog>
    {
        public BlogFilterDto()
        {
            base.PerPage = 32; // Sets default value when object is initialized.
            // Model binder will set property after constructor if request has this parameter.
        }
        public int? CategoryId { get; set; }

        public Range<int> Priority { get; set; }

        [StringFilterOptions(StringFilterOption.Contains)]
        public string Title { get; set; }

        public bool? IsPublished { get; set; }

        public Range<DateTime> PublishDate { get; set; }
    }
```