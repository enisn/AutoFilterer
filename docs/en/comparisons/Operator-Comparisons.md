# Operator Comparisons
Sometimes you may experience problems with database queries. All providers can not convert Linq to database query. Or just you may prefer use different operators with given parameter. So let's start:

- You have filter object like that:
```csharp
public class MyFilter : PaginationFilterBase
{
	public string Title { get; set; }
}
```

- And you want to use directly operator _(`==`, `!=`, `>`, `<`, `>=`, `<=`)_ comparison instead of `Equals()`, `Contains()` or something else methods.

- Just use `[OperatorComparison]` attribute with [OperatorType](/enisn/AutoFilterer/blob/master/src/AutoFilterer/Enums/OperatorType.cs) parameter.

```csharp
public class MyFilter : PaginationFilterBase
{
	[OperatorComparison(OperatorType.Equal)] // <-- This generates query with operator
	public string Title { get; set; }        // something like that: x => x.Title == "something"
}
```

- This attribute generates queries with exactly given operator.
For example when you use `OperatorType.Equal` this will generate query is 

`x => x.Title == "something"` 

But **StringFilterOptions** generates query as 

`x => x.Title.Equals("something", StringComparison.InvariantCultureIgnoreCase)`.

MongoDb doesn't support last one. So you can use **OperatorComparison** if you're using mongodb or something else which doesn't support `Equals` method.

_TODO: Refactor this page_