# StringFilter
`StringFilter` is a type that implements `IFilterableType`. So it can be used as a type of a property in Filter object. It's used for dynamic comparison. Both comparison type and comparison value are sent by client.

## Properties
StringFilter includes following properties;

- `Eq`: Provides parameter for equal operator `==` in query.
- `Not`: Provides parameter to not equal operator `!=` in query.
- `Equals`: Provides parameter to `String.Equals` method query.
- `Contains`: Provides parameter to `String.Contains` method query.
- `NotContains`: Provides parameter to `!String.Contains` method query.
- `StartsWith`: Provides parameter to `String.StartsWith` method query.
- `NotStartsWith`: Provides parameter to `!String.StartsWith` method query.
- `EndsWith`: Provides parameter to `String.EndsWith` method query.
- `NotEndsWith`: Provides parameter to `!String.EndsWith` method query.
- `IsNull`: Provides parameter to `== null` method query.
- `IsNotNull`: Provides parameter to `!= null` method query.
- `IsEmpty`: Provides parameter to `String.IsNullOrEmpty` method query.
- `IsNotEmpty`: Provides parameter to `!String.IsNullOrEmpty` method query.

## Usage
It can be used in the following way in filter object.

```csharp
public class BookFilter : FilterBase
{
    public StringFilter Title { get; set; }
}
```

That property can be used in the following ways in query.

`yourdomain.com/books?title.eq=Harry Potter`
`yourdomain.com/books?title.isEmpty=true`

> _All of the above properties can be used._



***
## What is next?
You may also see:
- [OperatorFilter](OperatorFilter)