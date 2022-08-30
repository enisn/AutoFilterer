# Migration to v2
If you're migrating from v1.x to v2.0, you should follow this documentation.

## Range
If you're using `Range<T>` you should change it because it's obsolete now. Replace it with `OperatorFilter<T>`.

But there is a **breaking change**: parameter names:

| `Range<T>` | `OperatorFilter<T>` |
| :---: | :---: |
| Min | Gte |
| Max | Lte |
| -   | Gt  |
| -   | Lt  |
| -   | eq  |
| -   | not |

To handle old client requests with same parameter name, you should override them:

```csharp
public class CustomRange<T> : OperatorFilter<T>
  where T : struct
{
    [FromQuery("Min")]
    public override T Gte { get; set; }
    [FromQuery("Max")]
    public override T Lte { get; set; }
}
```
Then, you can use CustomRange for migrating period.

