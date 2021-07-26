# Getting Started

1. Install [AutoFilterer](https://www.nuget.org/packages/AutoFilterer/) NuGet package.

   - Via CLI

     ```bash
     dotnet add package AutoFilterer
     ```

     

   - Package Reference

     ```xml
     <PackageReference Include="AutoFilterer" Version="2.*"/>
     ```



## AutoFilterer.Swagger

**OPTIONAL:** This step is not necessary to work with AutoFilterer but improves Swagger documentation.

1. Install [AutoFilterer.Swagger](https://www.nuget.org/packages/AutoFilterer.Swagger/) NuGet package into your Web project.

2. Go your **Startup** and add following namespace using:

   ```csharp
   using AutoFilterer.Swagger;
   ```

3. Find **AddSwaggerGen** method in your Startup and add `UseAutoFiltererParameters()` method like following:

   ```csharp
   services.AddSwaggerGen(c =>
   {
       c.UseAutoFiltererParameters(); // <-- Add this here.
   
       // ...
   });
   
   ```

   

---

## AutoFilterer.Generators

AutoFilterer.Generators aims to generate filter objects from entities automatically via using [dotnet source generators](https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/).

Visit [documentation of Generators](generators/AutoFilterer-Generators.md)