# Customizing Sorting Parameters
You may want to use different names instead of `sort` and `sortBy`

- Just you your filter object and override them:
```csharp
public class BlogFilterDto : PaginationFilterBase
{
    public int? CategoryId { get; set; }
    public OperatorFilter<int> Priority { get; set; }
    public StringFilter Title { get; set; }
    public bool? IsPublished { get; set; }
    public OperatorFilter<DateTime> PublishDate { get; set; }

    [FromQuery(Name = "order")] // <-- you can set querystring name
    public override Sorting SortBy{ get => base.Page; set => base.Page = value; }

    [FromQuery(Name = "by")]
    public override string Sort { get => base.PerPage; set => base.PerPage = value; }
}
```

- You can declare available sorting properties by manual with **PossibleSortings** attribute at the top of class:

```csharp
[PossibleSortings("Title", "Year","Language", "TotalPage")] // <-- Only these 4 fields can be sorted.
public class BlogFilterDto : PaginationFilterBase
{
    public int? CategoryId { get; set; }
    public OperatorFilter<int> Priority { get; set; }
    public StringFilter Title { get; set; }
    public bool? IsPublished { get; set; }
    public OperatorFilter<DateTime> PublishDate { get; set; }
}
```

To see these parameters in swagger you should use [AutoFilterer.Swagger](https://github.com/enisn/AutoFilterer/wiki/Set-Up#swagger-documentation) extension. After you initialize this library, all sorting parameters will be visible at swagger as **dropdown**.