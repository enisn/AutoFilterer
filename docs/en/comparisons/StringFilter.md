# StringFilter
It's used to compare string fields. `StringFilter` is a type that implements `IFilterableType`.So it can be used as a type of a property in Filter object. It's used for dynamic comparison. Both comparison type and comparison value are sent by client.

## Properties
StringFilter includes following properties;

- `Eq`: Provides parameter for `==` operator in expression.
- `Not`: Provides parameter for `!=` operator in expression.
- `Equals`: Provides parameter to `String.Equals` method expression.
- `Contains`: Provides parameter to `String.Contains` method expression.
- `NotContains`: Provides parameter to `!String.Contains` method expression.
- `StartsWith`: Provides parameter to `String.StartsWith` method expression.
- `NotStartsWith`: Provides parameter to `!String.StartsWith` method expression.
- `EndsWith`: Provides parameter to `String.EndsWith` method expression.
- `NotEndsWith`: Provides parameter to `!String.EndsWith` method expression.
- `IsNull`: Provides parameter to `== null` comparison in expression.
- `IsNotNull`: Provides parameter to `!= null` comparison in expression.
- `IsEmpty`: Provides parameter to `String.IsNullOrEmpty` method in expression.
- `IsNotEmpty`: Provides parameter to `!String.IsNullOrEmpty` method in expression.

## Usage
Use `StringFilter` as type of a property in Filter object.

```csharp
public class BookFilter : FilterBase
{
    public StringFilter Title { get; set; }
}
```

That property can be used in the following ways in querystring.

`yourdomain.com/books?title.eq=Harry Potter`
`yourdomain.com/books?title.isEmpty=true`

> _All of the above properties can be used._



***
## What is next?
You may also see:
- [OperatorFilter](OperatorFilter)