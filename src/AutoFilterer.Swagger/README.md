# Swagger Parameters
This library describes all AutoFilterer parameter into swagger.json and makes visible at Swashbuckle or any other swagger documentation tool.

# Set-up

- Install `AutoFilterer.Swagger` package from [NuGet](https://www.nuget.org/packages/AutoFilterer.Swagger)
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

# Showcase
After you set-up successfully, just launch the swagger and see parameters, they are shown as dropdown now 🚀

![image](https://user-images.githubusercontent.com/23705418/82148416-b89e0b80-985c-11ea-801c-646a1651dc3d.png)

