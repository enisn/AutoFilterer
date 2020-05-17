# Property Name Mapping
You may want to set which property from filter should be searched in which property in model. You can use `[CompareTo]` attribute to define property names in model.

- Property name/names can be defined via `CompareTo` attribute:

```csharp
public class BookFilter : FilterBase
{
    [CompareTo("Title","Author")] // <-- This filter will be applied to Title or Author.
    [StringFilterOptions(StringFilterOption.Contains)]
    public string Search { get; set; }
}
```

- You can define combination type between multiple parameters

```csharp
public class BookFilter : FilterBase
{
    [CompareTo("Title","Author", CombineWith = CombineType.And)] // <-- 'And'/'Or' can be set from here.
    [StringFilterOptions(StringFilterOption.Contains)]
    public string Search { get; set; }
}
```
