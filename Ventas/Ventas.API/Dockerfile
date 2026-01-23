FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Ventas/Ventas.API/Ventas.API.csproj", "Ventas/Ventas.API/"]
COPY ["Ventas/Ventas.Application/Ventas.Application.csproj", "Ventas/Ventas.Application/"]
COPY ["Ventas/Ventas.Domain/Ventas.Domain.csproj", "Ventas/Ventas.Domain/"]
COPY ["Ventas/Ventas.Infrastructure/Ventas.Infrastructure.csproj", "Ventas/Ventas.Infrastructure/"]
COPY ["Shared/Facturacion.Shared/Facturacion.Shared.csproj", "Shared/Facturacion.Shared/"]
RUN dotnet restore "Ventas/Ventas.API/Ventas.API.csproj"
COPY . .
WORKDIR /src/Ventas/Ventas.API

FROM build AS publish
WORKDIR /src/Ventas/Ventas.API
RUN dotnet publish "Ventas.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ventas.API.dll"]
