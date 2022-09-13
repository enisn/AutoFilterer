# String Comparisons

AutoFilterer provides a strong API to define string comparisons in the DTOs.
`StringFilterOptions` attribute is used to define a comparison. The attribute is optional. Default value is `Equals`.

## Options
There are 4 options available for string comparisons:

- `Equals`: Generates a filter that checks if the value is equal to the specified value.
  ```csharp
    [StringFilterOptions(StringComparison.Equals)]
    public string Name { get; set; }
    ```

    Generated LINQ expression: `x => Name == "John"`

- `StartsWith`: Generates a filter that checks if the value starts with the specified value.
    ```csharp
    [StringFilterOptions(StringComparison.StartsWith)]
    public string Name { get; set; }
    ```
    Generated LINQ expression: `x => Name.StartsWith("John")`

- `EndsWith`: Generates a filter that checks if the value ends with the specified value.
    ```csharp
    [StringFilterOptions(StringComparison.EndsWith)]
    public string Name { get; set; }
    ```
    Generated LINQ expression: `x => Name.EndsWith("John")`
- `Contains`: Generates a filter that checks if the value contains the specified value.
    ```csharp
    [StringFilterOptions(StringComparison.Contains)]
    public string Name { get; set; }
    ```
    Generated LINQ expression: `x => Name.Contains("John")`

## Comparisons
`System.StringComparison` is used to define the comparison type as a second parameter. It can be used to filter case-insensitive queries.

```csharp
[StringFilterOptions(StringFilterOption.Contains, StringComparison.InvariantCultureIgnoreCase)]
public string Name { get; set; }
```

Generated LINQ expression: `x => Name.Contains("John", StringComparison.InvariantCultureIgnoreCase)`

## Common Issues
Database providers don't support same logic while generating database queries from LINQ expressions. `StringComparison.InvariantCultureIgnoreCase` is not supported by all database providers. As a workaround, you can use `ToLower()` method and database provider accepts it's a case insensitive query. MongoDB and SqlServer uses that logic to generate case-insensitive queries.

Achieving `ToLower()` expression generation, you need to use `ToLowerContainsComparison` attribute. It'll generate a a proper `x => x.Name.ToLower().Contains("John")` expression.

```csharp
    [ToLowerContainsComparison]
    public string Name { get; set; }
```

