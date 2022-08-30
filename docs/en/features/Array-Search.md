# Array Search
Array search, provides filtering with multiple parameters for single property. Just make your filter object property as array and see the magic!

## Showcase

- Entity

```csharp
// Entity
public class Book 
{
	public string Id { get; set; }
	public string Title { get; set; }
        public PublishStatus Status { get; set; }
}

public enum PublishStatus
{
    Waiting,
    Rejected,
    Reviewed,
    Published,
}
```
- Filter object
```csharp
public class BookFilter : PaginationFilterBase
{
	public PublishStatus[] Status { get; set; } // <-- Make this array, see the magic!
}
```

- Swagger UI will show it as multi-selectable field:
![image](https://user-images.githubusercontent.com/23705418/97888028-b6146d00-1d3b-11eb-81ed-85dc4cfa726d.png)


--- 

Also you can use `[ArraySearchFilter]` attribute if your field type is not array but IEnumerable:
```csharp
public class BookFilter : PaginationFilterBase
{
	[ArraySearchFilter] // <-- Use this attribute if your field is not array.
	public List<PublishStatus> Status { get; set; }
}
```

