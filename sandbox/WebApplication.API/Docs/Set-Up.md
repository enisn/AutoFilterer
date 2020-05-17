# Getting Started

- Install `AutoFilterer` NuGet package into your project from [here](https://www.nuget.org/packages/AutoFilterer/).

*That's it. You don't need any initialization*

You may see next [Basics](Basics)

---

## Swagger Documentation
**OPTIONAL:** This step is not necessary to work with AutoFilterer but improves Swagger documentation.

- Install `AutoFilterer.Swagger` NuGet package into your project from [here](https://www.nuget.org/packages/AutoFilterer/).

- Go your **Startup** and add following namespace using:
```csharp
using AutoFilterer.Swagger;
```

- Find **AddSwaggerGen** in your Startup and add `UseAutoFiltererParameters()` method like following:

```csharp
services.AddSwaggerGen(c =>
{
    c.UseAutoFiltererParameters(); // <-- Add this here.

    // ...
});
```