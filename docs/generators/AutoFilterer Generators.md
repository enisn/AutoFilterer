# AutoFilterer Generators

AutoFilterer.Generators aims to generate filter objects automatically from entities via using [dotnet source generators]([Introducing C# Source Generators | .NET Blog (microsoft.com)](https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/)). 

> **WARNING:** This feature is beta for now and might will be have braking changes future.

## Usage

- Install `AutoFilterer.Generators` from NuGet.

- Find your Entity or Model you want to create auto filter object for.

- Add `[AutoFilterDto]` attribute over it:

  ```csharp
  [AutoFilterDto]
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
  


## Features

Generators tries to create best filter object that fit you requirements. 

So it uses a couple of mappings:

- Numeric properties will be created as `Range<T>`
- String properties will be created as `string` with `[ToLowerContains]` attribute
- DateTime properties will be created as `Range<DateTime>` 
- Complex Types aren't supported yet.



Namespace can be customized with attribute parameter.

```csharp
[AutoFilterDto("My.SpecialNamespace.Filters")]
public class MyAwesomeEntity
{
  /...
}
```

