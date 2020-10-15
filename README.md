<table>
<tr>
<td width="15%">

 ![Logo](https://github.com/enisn/AutoFilterer/blob/master/content/auto_filterer_icon.png?raw=true) 

</td>
<td>

 # AutoFilterer

AutoFilterer is a mini filtering & querying framework for .NET Standard. The project aims applying automatic filters in Open API 3.0 specifications unlike oData or graphql. All parameters are supported Open API 3.0 specifications and REST.
As summary, this project aims to create queries without writing any of query with **IQueryable**. Just prepare your filter model and apply it into your Db Entity.

Generated queries supports Entity Framework and MongoDB. 

> This library **does not** generate directly database queries. It generates LINQ Expressions.

You can visit [Wiki](../../wiki) for more documents

[![Nuget](https://img.shields.io/nuget/v/AutoFilterer?logo=nuget)](https://www.nuget.org/packages/AutoFilterer/)
[![WiKi](https://img.shields.io/badge/Visit-Wiki-orange)](../../wiki)
[![CodeFactor](https://www.codefactor.io/repository/github/enisn/autofilterer/badge)](https://www.codefactor.io/repository/github/enisn/autofilterer)
[![Build status](https://ci.appveyor.com/api/projects/status/fhsry13a6k6j712w?svg=true)](https://ci.appveyor.com/project/enisn/autofilterer)
<a href="https://gitmoji.carloscuesta.me">
  <img src="https://img.shields.io/badge/gitmoji-%20😜%20😍-FFDD67.svg?style=flat-square" alt="Gitmoji">
</a>
</td>
</tr>
</table>


# Getting Started

- Install `AutoFilterer` NuGet package from [here](https://www.nuget.org/packages/AutoFilterer/).

You may visit [visit documentation](../../wiki) for better understanding of implementation.


***

# Showcase

You can TRY [LIVE DEMO](https://autofilterer-showcase.herokuapp.com/swagger/index.html#/Books/get_api_Books).

- All querying supports Open API 3.0 Specifications:

![image](https://user-images.githubusercontent.com/23705418/82128447-f9961180-97c3-11ea-87b3-452c38d9f676.png)

- And Result:

![image](https://user-images.githubusercontent.com/23705418/82128521-6ad5c480-97c4-11ea-9f78-575733c101dd.png)

***

# Usage
Just [visit Wiki](../../wiki) for better understanding of usage.

## Swagger
All parameters suppors OpenAPI 3.0 Specifications 👍

Improve your swagger documentation via using [AutoFilterer.Swagger](https://www.nuget.org/packages/AutoFilterer.Swagger)
