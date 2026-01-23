FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Inventario/Inventario.API/Inventario.API.csproj", "Inventario/Inventario.API/"]
COPY ["Inventario/Inventario.Application/Inventario.Application.csproj", "Inventario/Inventario.Application/"]
COPY ["Inventario/Inventario.Domain/Inventario.Domain.csproj", "Inventario/Inventario.Domain/"]
COPY ["Inventario/Inventario.Infrastructure/Inventario.Infrastructure.csproj", "Inventario/Inventario.Infrastructure/"]
COPY ["Shared/Facturacion.Shared/Facturacion.Shared.csproj", "Shared/Facturacion.Shared/"]
RUN dotnet restore "Inventario/Inventario.API/Inventario.API.csproj"
COPY . .
WORKDIR /src/Inventario/Inventario.API

FROM build AS publish
WORKDIR /src/Inventario/Inventario.API
RUN dotnet publish "Inventario.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Inventario.API.dll"]
