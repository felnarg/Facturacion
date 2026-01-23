FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Compras/Compras.API/Compras.API.csproj", "Compras/Compras.API/"]
COPY ["Compras/Compras.Application/Compras.Application.csproj", "Compras/Compras.Application/"]
COPY ["Compras/Compras.Domain/Compras.Domain.csproj", "Compras/Compras.Domain/"]
COPY ["Compras/Compras.Infrastructure/Compras.Infrastructure.csproj", "Compras/Compras.Infrastructure/"]
COPY ["Shared/Facturacion.Shared/Facturacion.Shared.csproj", "Shared/Facturacion.Shared/"]
RUN dotnet restore "Compras/Compras.API/Compras.API.csproj"
COPY . .
WORKDIR /src/Compras/Compras.API

FROM build AS publish
WORKDIR /src/Compras/Compras.API
RUN dotnet publish "Compras.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Compras.API.dll"]
