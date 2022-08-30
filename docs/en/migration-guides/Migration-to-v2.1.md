# Migration to v2.1
If you're migrating from v2.0.x to v2.1.x, you should follow this documentation.

## OrderBy
Sorting feature is added at v2.1 and if you're already making custom Ordering before **ApplyFilter()** method, you should remove it to prevent apply OrderBy double time.

- If you have
```csharp
db.Books.OrderByDescending(o => o.Year).ApplyFilter(filter).ToList();
```

- You may change

```csharp
db.Books.ApplyFilter(filter).ToList();
```

And your filter object:
```csharp
public class BookFilter : PaginationFilterBase
{
    public BookFilter()
    {
        // Set the default values in ctor or you can override them.
        Sort = nameof(Book.Year);
        SortBy = Sorting.Descending;
    }

    public StringFilter Title { get; set; }

    [StringFilterOptions(StringFilterOption.Contains)]
    public string Language { get; set; }

    public StringFilter Author { get; set; }

    public OperatorFilter<int> TotalPage { get; set; }

    public OperatorFilter<int> Year { get; set; }
}
```

- And they can be sent different values in querystring like:

    [/books?sort=TotalPage&sortBy=Ascending](https://autofilterer-showcase.azurewebsites.net/api/Books?sort=Year&sortBy=Descending)