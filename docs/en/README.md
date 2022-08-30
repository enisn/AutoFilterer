 # AutoFilterer

AutoFilterer is a filtering framework for dotnet.
The main purpose of this library is to generate LINQ expressions from DTOs for entities automatically. Creating queries without writing any of LINQ Expression for 'IQueryable' is provided by AutoFilterer. All parameters and usage, target to be compatible with Open API 3.0 Specifications, unlike oData & GraphQL.



## Index

- [Getting Started](Getting-Started.md)
- [Live Demo](https://autofilterer-showcase.herokuapp.com/swagger/index.html#/Books/get_api_Books).

## AutoFilterer.Swagger
Swagger extension of AutoFilterer improves swagger documentation and converts supported text fields to dropdowns.

See [AutoFilterer.Swagger](https://github.com/enisn/AutoFilterer/tree/master/src/AutoFilterer.Swagger)



## AutoFilterer.Generators

[AutoFilterer.Generators](generators/AutoFilterer-Generators.md) aims to generate filter objects from entities automatically via using [dotnet source generators](https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/). 



## AutoFilterer.Dynamics

Dynamics extension of AutoFilterer aims to use AutoFilterer without types. Core AutoFilterer feature uses classes and keeps your code type-safe but Dynamics extension uses dictionaries and plain json.