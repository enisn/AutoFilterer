# OperatorFilter
It's used to compare types that implements `IComparable` interface. _(Mostly numeric fields)_. `OperatorFilter` is a type that implements `IFilterableType`.So it can be used as a type of a property in Filter object. It's used for dynamic comparison. Both comparison type and comparison value are sent by client.

## Properties
OperatorFilter includes following properties;

- `Eq`: Provides parameter for equal operator `==` in expression.
- `Not`: Provides parameter to not equal operator `!=` in expression.
- `Gt`: Provides parameter to greater than operator `>` in expression.
- `Lt`: Provides parameter to less than operator `<` in expression.
- `Gte`: Provides parameter to greater than or equal operator `>=` in expression.
- `Lte`: Provides parameter to less than or equal operator `<=` in expression.
- `IsNull`: Provides parameter to `== null` comparison in expression.
- `IsNotNull`: Provides parameter to `!= null` comparison inexpression.

## Usage
Use `OperatorFilter` as type of a property in Filter object.

```csharp
public class BookFilter : FilterBase
{
    public OperatorFilter<int> TotalPage { get; set; }
}
```

That property can be used in the following ways in querystring.

`/books?totalPage.gte=300`
`/books?totalPage.isNull=true`
`/books?totalPage.gte=120&totalPage.lte=200`

## Further reading
- [StringFilter](StringFilter)