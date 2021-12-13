#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["sandbox/WebApplication.API/WebApplication.API.csproj", "sandbox/WebApplication.API/"]
COPY ["src/AutoFilterer.Dynamics/AutoFilterer.Dynamics.csproj", "src/AutoFilterer.Dynamics/"]
COPY ["src/AutoFilterer/AutoFilterer.csproj", "src/AutoFilterer/"]
COPY ["src/AutoFilterer.Swagger/AutoFilterer.Swagger.csproj", "src/AutoFilterer.Swagger/"]
COPY ["src/AutoFilterer.Generators/AutoFilterer.Generators.csproj", "src/AutoFilterer.Generators/"]
RUN dotnet restore "sandbox/WebApplication.API/WebApplication.API.csproj"
COPY . .
WORKDIR "/src/sandbox/WebApplication.API"
RUN dotnet build "WebApplication.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApplication.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "WebApplication.API.dll"]
CMD ASPNETCORE_URLS=http://*:$PORT dotnet WebApplication.API.dll