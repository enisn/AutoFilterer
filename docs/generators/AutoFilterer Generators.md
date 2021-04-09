# AutoFilterer Generators

AutoFilterer.Generators aims to generate filter objects automatically from entities via using [dotnet source generators](https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/). 

> **WARNING:** This feature is beta for now and might will be have braking changes future.

## Usage

- Install [AutoFilterer.Generators](https://www.nuget.org/packages/AutoFilterer.Generators) from NuGet.

- Find your Entity you want to create auto filter object for.

- Add `[GenerateAutoFilter]` attribute over it:

  ```csharp
  [GenerateAutoFilter]
  public class Book
  {
    public string Title { get; set; }
    public int? Year { get; set; }
    public int TotalPage { get; set; }
    public DateTime PublishTime { get; set; }
  }
  ```

  

- You'll see `BookFilter` objects exists in your project.  You can start to use it. Ready to use get from query directly:

  ```csharp
  public Task<IActionResult> GetAsync([FromQuery] BookFilter filter)
  {
    return Ok(db.Books.ApplyFilter(filter));
   	// or place your fancy code instead of my ugly line :)
  }
  ```
  
  BookFilter source code looks like:
  
  ```csharp
  public partial class BookFilter : PaginationFilterBase
  {
      public string Title { get; set; }
      public Range<int> Year { get; set; }
      public Range<int> TotalPage { get; set; }
      public Range<System.DateTime> PublishTime { get; set; }
  }
  ```

## Features

Generators tries to create best filter object that fit you requirements. 

- So it uses a couple of mappings:
  - Numeric properties will be created as `Range<T>`
  - String properties will be created as `string` with `[ToLowerContains]` attribute
  - DateTime properties will be created as `Range<DateTime>` 
  - Complex Types aren't supported yet.

### Namespace

Namespace can be customized with attribute parameter.

```csharp
[GenerateAutoFilter("My.SpecialNamespace.Filters")]
public class MyAwesomeEntity
{
  /...
}
```

### Extensibility

 All generated classes are partial and have virtual members. So you have 2 option to extend class:

- **Partial Class**: A partial class can be created with same name in same namespace. So you can add more members in it.

  ```csharp
  namespace My.SpecialNamespace.Filters
  {
    public partial MyAwesomeEntity
    {
      // You can add new members. 
      public string ExtraParameter { get; set; }
    	
      // Also you can create constructor.
      public MyAwesomeEntity()
      {
        // And set default values.
        SortBy = Sorting.Descending;
        PerPage = 36;
      }
    }
  }
  ```

  

- **Inheritance**: Custom filter classes can added  which inherits from Auto Generated types to override members.

  ```csharp
  public class AdvancedBookFilter : BookFilter
  {
    [FromQuery(Name = "p")]
    public override int Page { get; set; }
  
    [FromQuery(Name = "size")]
    public override int PerPage { get; set; }
  }
  ```

  
