FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["sandbox/WebApplication.API/WebApplication.API.csproj", "sandbox/WebApplication.API/"]
RUN dotnet restore "sandbox/WebApplication.API/WebApplication.API.csproj"
COPY . .
WORKDIR "/src/sandbox/WebApplication.API"
RUN dotnet build "WebApplication.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "WebApplication.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebApplication.API.dll"]