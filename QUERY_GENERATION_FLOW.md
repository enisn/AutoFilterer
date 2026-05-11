# AutoFilterer Query Generation Flow

## Overview

AutoFilterer is a library that provides a declarative and attribute-driven approach to building complex LINQ queries for filtering, ordering, and pagination. This document explains how queries are generated from filter objects using the `ApplyFilter<T>()` method as the entry point.

## Architecture Overview

The query generation in AutoFilterer follows a **layered architecture** with distinct responsibilities:

1. **Entry Point Layer** - `QueryExtensions.ApplyFilter<T>()`
2. **Filter Processing Layer** - `IFilter` interface and `FilterBase` class
3. **Expression Building Layer** - Attribute-based expression builders
4. **Ordering & Pagination Layer** - `IOrderable` and `IPaginationFilter` interfaces
5. **LINQ Execution Layer** - Final query execution

## 1. Entry Point: QueryExtensions.ApplyFilter<T>()

### Location
[Extensions/QueryExtensions.cs](src/AutoFilterer/Extensions/QueryExtensions.cs)

### Purpose
The `ApplyFilter<T>()` method serves as the public API gateway for applying filters to LINQ queries.

### Overloads

```csharp
// Works with unordered IQueryable<T>
public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> source, IFilter filter)
{
    return filter.ApplyFilterTo(source);
}

// Works with already ordered IOrderedQueryable<T>
public static IQueryable<T> ApplyFilter<T>(this IOrderedQueryable<T> source, IFilter filter)
{
    return filter.ApplyFilterTo(source);
}
```

### Key Characteristics
- **Generic Method**: Works with any entity type `T`
- **Extension Method**: Seamlessly integrates with LINQ query chains
- **Chainable**: Allows combining with other LINQ operations
- **Delegation Pattern**: Delegates actual filter logic to the filter object's `ApplyFilterTo<TEntity>()` method

### Example Usage

```csharp
// Basic filtering
var books = db.Books
    .ApplyFilter(filter)
    .ToList();

// With additional Where clauses
var books = db.Books
    .Where(x => !x.IsDeleted)
    .ApplyFilter(filter)
    .Select(s => s.Name)
    .ToList();
```

## 2. Filter Processing Layer

### IFilter Interface

```csharp
public interface IFilter
{
    Expression BuildExpression(Type entityType, Expression body);
    IQueryable<TEntity> ApplyFilterTo<TEntity>(IQueryable<TEntity> query);
}
```

### Core Components

#### FilterBase Class
The base class for all filter implementations. It contains the core logic for:
1. **Expression Building** - Creating LINQ expressions from filter properties
2. **Query Transformation** - Converting expressions into WHERE clauses
3. **Property Reflection** - Scanning filter object for filterable properties

### Query Generation Flow

```
ApplyFilter<T>()
    ↓
IFilter.ApplyFilterTo<TEntity>()
    ↓
FilterBase.ApplyFilterTo<TEntity>()
    ├─ Create parameter expression: Expression.Parameter(typeof(T), "x")
    ├─ Build filter expression via BuildExpression()
    ├─ Create lambda: Expression.Lambda<Func<T, bool>>(exp, parameter)
    └─ Apply WHERE: query.Where(lambda)
```

## 3. Expression Building Layer

### 3.1 BuildExpression Method

The heart of the filter system. This method:

1. **Iterates** through all properties of the filter object
2. **Reflects** on property types and attributes
3. **Builds** individual filter expressions for each property
4. **Combines** expressions using AND/OR logic

### Detailed Flow

```
FilterBase.BuildExpression(Type entityType, Expression body)
    ↓
For each property in filter class:
    ├─ Get property value
    ├─ Skip null values and properties with [IgnoreFilter]
    ├─ Get [CompareTo] or [FilteringOptions] attributes
    ├─ For each attribute:
    │   └─ BuildExpressionForProperty() → Creates expression
    └─ Combine expressions with CombineType (AND/OR)
```

### ExpressionBuildContext

The context object that carries all necessary information for building an expression:

```csharp
public class ExpressionBuildContext
{
    public Expression ExpressionBody { get; }              // Current expression body (e.g., "x")
    public PropertyInfo TargetProperty { get; }           // Property on entity being filtered
    public PropertyInfo FilterProperty { get; }           // Property on filter object
    public Expression FilterPropertyExpression { get; }   // Expression for filter value
    public IFilter FilterObject { get; }                  // The filter object instance
    public object FilterObjectPropertyValue { get; }      // Actual value of filter property
}
```

## 4. Attribute-Based Expression Builders

Different attributes generate different types of expressions:

### 4.1 CompareToAttribute

**Purpose**: Maps filter properties to entity properties

```csharp
[CompareTo("PropertyName")]
public string Name { get; set; }
```

**Behavior**:
- Creates equality comparisons by default
- Can map to multiple properties (OR combined)
- Can use custom `IFilterableType` implementations

### 4.2 OperatorComparisonAttribute

**Purpose**: Creates comparison expressions (>, <, >=, <=, ==, !=, null checks)

**Operators**:
- `Equal` - `==`
- `NotEqual` - `!=`
- `GreaterThan` - `>`
- `GreaterThanOrEqual` - `>=`
- `LessThan` - `<`
- `LessThanOrEqual` - `<=`
- `IsNull` - `== null`
- `IsNotNull` - `!= null`

**Expression Generation**:
```csharp
// For GreaterThan
Expression.GreaterThan(
    Expression.Property(context.ExpressionBody, propertyName),
    Expression.Constant(filterValue)
)
```

### 4.3 StringFilterOptionsAttribute

**Purpose**: String-specific filtering (Contains, StartsWith, EndsWith)

**Options**:
- `Contains` - `String.Contains(value)`
- `StartsWith` - `String.StartsWith(value)`
- `EndsWith` - `String.EndsWith(value)`

**Expression Generation**:
```csharp
// For Contains with case-insensitive comparison
Expression.Call(
    method: typeof(string).GetMethod("Contains", new[] { typeof(string), typeof(StringComparison) }),
    instance: Expression.Property(context.ExpressionBody, propertyName),
    arguments: new[] { filterValue, Expression.Constant(StringComparison.InvariantCultureIgnoreCase) }
)
```

### 4.4 CollectionFilterAttribute

**Purpose**: Filters nested collections using Any() or All()

**Filter Options**:
- `Any` - At least one item matches
- `All` - All items match

**Expression Generation**:
```csharp
// For collection.Any(item => condition)
Expression.Call(
    method: typeof(Enumerable).GetMethod("Any"),
    instance: null,
    arguments: new[] { 
        Expression.Property(context.ExpressionBody, collectionPropertyName),
        innerLambda  // Lambda with nested filter conditions
    }
)
```

## 5. FilterableType Pattern

### IFilterableType Interface

Used for complex type filtering. Custom types can implement this to define their own filter logic.

### Built-in FilterableTypes

#### StringFilter
Provides rich string filtering options:

```csharp
public class StringFilter : IFilterableType
{
    public string Eq { get; set; }           // Exact match
    public string Not { get; set; }          // Not equal
    public string Contains { get; set; }     // Contains substring
    public string NotContains { get; set; }  // Doesn't contain
    public string StartsWith { get; set; }   // Starts with
    public string NotStartsWith { get; set; }
    public string EndsWith { get; set; }     // Ends with
    public string NotEndsWith { get; set; }
    public bool? IsNull { get; set; }        // Null check
    public bool? IsNotNull { get; set; }     // Not null
    public bool? IsEmpty { get; set; }       // Empty string
    public bool? IsNotEmpty { get; set; }    // Not empty
    public StringComparison? Compare { get; set; }
}
```

#### OperatorFilter<T>
Provides rich numeric/comparable type filtering:

```csharp
public class OperatorFilter<T> : IFilterableType where T : struct
{
    public T? Eq { get; set; }      // Equal
    public T? Not { get; set; }     // Not equal
    public T? Gt { get; set; }      // Greater than
    public T? Lt { get; set; }      // Less than
    public T? Gte { get; set; }     // Greater than or equal
    public T? Lte { get; set; }     // Less than or equal
    public bool? IsNull { get; set; }
    public bool? IsNotNull { get; set; }
}
```

### Range Type

For filtering numeric ranges:

```csharp
public class Range : IRange
{
    public T? Start { get; set; }
    public T? End { get; set; }
}
```

**Filter Expression**:
```
property >= Start AND property <= End
```

## 6. Expression Combination

### CombineType Enum

Determines how multiple conditions are joined:

```csharp
public enum CombineType
{
    And,  // && operator
    Or    // || operator
}
```

### ExpressionExtensions.Combine()

Intelligently combines expressions:

```csharp
public static Expression Combine(this Expression left, Expression right, CombineType combineType)
{
    if (left == null) return right;
    if (right == null) return left;
    
    // Skip parameter and member expressions
    if (left is ParameterExpression || left is MemberExpression) return right;
    if (right is ParameterExpression || right is MemberExpression) return left;
    
    // Combine with AND or OR
    if (combineType == CombineType.And)
        return Expression.AndAlso(left, right);
    else
        return Expression.OrElse(left, right);
}
```

### Example Expression Combination

```csharp
// Filter: Name = "John" OR Name = "Jane"
Expression 1: x => x.Name == "John"
Expression 2: x => x.Name == "Jane"
Combine(Expr1, Expr2, Or) → x => (x.Name == "John") || (x.Name == "Jane")

// Filter: Age > 18 AND Status = Active
Expression 1: x => x.Age > 18
Expression 2: x => x.Status == Active
Combine(Expr1, Expr2, And) → x => (x.Age > 18) && (x.Status == Active)
```

## 7. Ordering Layer

### IOrderable Interface

```csharp
public interface IOrderable
{
    Sorting SortBy { get; set; }  // Ascending or Descending
    string Sort { get; }           // Property name to sort by
    IOrderedQueryable<TSource> ApplyOrder<TSource>(IQueryable<TSource> source);
}
```

### OrderableBase Implementation

```
ApplyOrder() method:
    ├─ Validate Sort property name is not empty
    ├─ Create parameter expression for entity
    ├─ Build member expression from property name (supports nested: "Author.Name")
    ├─ Create property lambda
    ├─ Check [PossibleSortings] attribute for allowed properties
    └─ Apply OrderBy or OrderByDescending via reflection
```

### Sorting Enum

```csharp
public enum Sorting
{
    Ascending,
    Descending
}
```

### Expression Generation for Ordering

```csharp
// For Ascending order
var lambda = Expression.Lambda(property, parameter);  // x => x.PropertyName
orderBy.MakeGenericMethod(typeof(TEntity), propertyType)
    .Invoke(null, new[] { source, lambda });

// For Descending order
var lambda = Expression.Lambda(property, parameter);
orderByDescending.MakeGenericMethod(typeof(TEntity), propertyType)
    .Invoke(null, new[] { source, lambda });
```

## 8. Pagination Layer

### IPaginationFilter Interface

```csharp
public interface IPaginationFilter : IFilter
{
    int Page { get; set; }      // 1-based page number
    int PerPage { get; set; }   // Items per page
    IQueryable<T> ApplyFilterWithoutPagination<T>(IQueryable<T> query);
}
```

### PaginationFilterBase Implementation

```
ApplyFilterTo<TEntity>():
    ├─ Check if query is already ordered
    ├─ If yes: Apply filters → Apply ordering → Apply pagination
    └─ If no: Apply filters → Apply pagination
    
ApplyFilterWithoutPagination<T>():
    └─ Applies only WHERE conditions (no Skip/Take)
```

### Pagination Logic

```csharp
public IQueryable<T> ToPaged<T>(this IQueryable<T> source, int page, int pageSize)
{
    // Skip: (page - 1) * pageSize
    // Take: pageSize
    return source.Skip((page - 1) * pageSize).Take(pageSize);
}
```

**Example**:
```
Page 1, PerPage 10: Skip(0).Take(10)    → Items 1-10
Page 2, PerPage 10: Skip(10).Take(10)   → Items 11-20
Page 3, PerPage 10: Skip(20).Take(10)   → Items 21-30
```

## 9. Complete Query Generation Flow

```
User Code
    ↓
db.Books.ApplyFilter(bookFilter)
    ↓
IFilter.ApplyFilterTo<Book>(IQueryable<Book>)
    ↓
[Branch on Filter Type]
    ├─ FilterBase: Apply WHERE only
    ├─ OrderableFilterBase: Apply WHERE → Apply ORDER BY
    └─ PaginationFilterBase: Apply WHERE → Apply ORDER BY → Apply SKIP/TAKE
    
[For FilterBase]
    ├─ Create parameter: Expression.Parameter(typeof(Book), "x")
    └─ BuildExpression(typeof(Book), parameter)
        ├─ Iterate filter properties: [Title, Author, YearPublished, Status]
        ├─ For each property with value:
        │   ├─ Get attributes: [CompareTo], [OperatorComparison], etc.
        │   ├─ Build expression for each attribute
        │   └─ Combine with AND/OR
        └─ Create lambda: Expression.Lambda<Func<Book, bool>>(expression, parameter)
    
    └─ Execute: query.Where(lambda)

[For OrderableFilterBase - if Sort is provided]
    ├─ ApplyFilter() → OrderedQueryable<T>
    └─ ApplyOrder()
        ├─ Parse Sort property name
        ├─ Create member expression
        └─ Execute OrderBy/OrderByDescending via reflection

[For PaginationFilterBase - if Page/PerPage provided]
    ├─ Apply filters and ordering
    └─ Apply ToPaged()
        └─ Skip((Page-1) * PerPage).Take(PerPage)

    ↓
Final LINQ Expression Tree
    ↓
.ToList() / .FirstOrDefault() / etc.
    ↓
Database Query Execution
    ↓
Results
```

## 10. Complete Example

### Filter Definition

```csharp
public class BookFilter : PaginationFilterBase
{
    [CompareTo(nameof(Book.Title))]
    [StringFilterOptions(StringFilterOption.Contains)]
    public string Title { get; set; }

    [CompareTo(nameof(Book.Author))]
    public OperatorFilter<string> Author { get; set; }

    [CompareTo(nameof(Book.YearPublished))]
    public OperatorFilter<int> YearPublished { get; set; }

    [CompareTo(nameof(Book.Categories))]
    [CollectionFilter(CollectionFilterType.Any)]
    public CategoryFilter Categories { get; set; }

    [PossibleSortings(nameof(Book.Title), nameof(Book.YearPublished))]
    public override string Sort { get; set; }
}

public class CategoryFilter : FilterBase
{
    [CompareTo(nameof(Category.Name))]
    public StringFilter Name { get; set; }
}
```

### Query Execution

```csharp
var filter = new BookFilter
{
    Title = "LINQ",                        // Contains "LINQ"
    Author = new OperatorFilter<string>   // Author starts with "E"
    {
        Gt = "E",
        Lte = "F"
    },
    YearPublished = new OperatorFilter<int>  // Published after 2010
    {
        Gte = 2010
    },
    Categories = new CategoryFilter
    {
        Name = new StringFilter { Contains = "Technology" }
    },
    Sort = nameof(Book.YearPublished),    // Order by year
    SortBy = Sorting.Descending,          // Descending
    Page = 1,                              // First page
    PerPage = 20                           // 20 per page
};

var results = db.Books.ApplyFilter(filter).ToList();
```

### Generated SQL (Conceptual)

```sql
SELECT * FROM Books
WHERE 
    Title LIKE '%LINQ%'
    AND Author > 'E' AND Author <= 'F'
    AND YearPublished >= 2010
    AND Categories.ANY(c => c.Name LIKE '%Technology%')
ORDER BY YearPublished DESC
OFFSET 0 ROWS
FETCH NEXT 20 ROWS ONLY
```

### Generated LINQ Expression (Conceptual)

```csharp
db.Books
    .Where(x => 
        x.Title.Contains("LINQ") &&
        x.Author > "E" && x.Author <= "F" &&
        x.YearPublished >= 2010 &&
        x.Categories.Any(c => c.Name.Contains("Technology"))
    )
    .OrderByDescending(x => x.YearPublished)
    .Skip(0)
    .Take(20)
    .ToList()
```

## 11. Key Design Patterns

### 1. **Visitor Pattern**
The filter object visits each property, determines its type, and builds corresponding expressions.

### 2. **Strategy Pattern**
Different attributes implement different filtering strategies (string matching, numeric comparison, collection filtering).

### 3. **Expression Trees**
LINQ Expression Trees allow compile-time query building that translates to efficient SQL.

### 4. **Template Method Pattern**
`FilterBase.ApplyFilterTo()` defines the structure; `BuildExpression()` is overridden by subclasses.

### 5. **Decorator Pattern**
`OrderableFilterBase` and `PaginationFilterBase` decorate `FilterBase` to add ordering and pagination.

### 6. **Reflection**
Properties are discovered at runtime using reflection, allowing automatic binding of filter properties to entity properties.

## 12. Error Handling and Validation

### Exception Handling

By default, exceptions during filter building are suppressed:

```csharp
public static bool IgnoreExceptions { get; set; } = true;
```

Can be set to `false` to debug filter issues.

### Validation Points

1. **Null Checks**: Null property values are skipped
2. **Property Existence**: Non-existent entity properties are skipped gracefully
3. **Sorting Validation**: `[PossibleSortings]` attribute restricts allowed sort fields
4. **Pagination Validation**: Page and PageSize must be positive integers

## 13. Performance Considerations

### Expression Caching

- **Formatter Cache**: String format expressions are cached in `ConcurrentDictionary`
- **Expression Compilation**: Lambda expressions are compiled once and reused

### Lazy Evaluation

- Expressions are built but not executed until `.ToList()`, `.FirstOrDefault()`, etc.
- This allows the database to handle filtering, not application memory

### Reflection Overhead

- Property reflection happens at filter application time
- For high-performance scenarios, consider caching filter schema

## 14. Extension Points

### Custom IFilterableType Implementation

```csharp
public class CustomRangeFilter : IFilterableType
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public Expression BuildExpression(ExpressionBuildContext context)
    {
        Expression expression = null;

        if (StartDate.HasValue)
            expression = Expression.GreaterThanOrEqual(
                Expression.Property(context.ExpressionBody, context.TargetProperty.Name),
                Expression.Constant(StartDate.Value)
            );

        if (EndDate.HasValue)
        {
            var endExpression = Expression.LessThanOrEqual(
                Expression.Property(context.ExpressionBody, context.TargetProperty.Name),
                Expression.Constant(EndDate.Value)
            );
            expression = expression?.Combine(endExpression, CombineType.And) ?? endExpression;
        }

        return expression;
    }
}
```

## 15. Summary

The AutoFilterer query generation flow is an elegant system that:

1. **Takes filter objects** as declarative specifications
2. **Uses reflection and attributes** to discover filter requirements
3. **Builds LINQ expressions** that represent WHERE, ORDER BY, and pagination clauses
4. **Combines expressions** intelligently with AND/OR logic
5. **Delegates to LINQ** for final query execution and database translation

This architecture provides a **type-safe, maintainable, and flexible** approach to building complex queries while keeping business logic separated from query construction.
