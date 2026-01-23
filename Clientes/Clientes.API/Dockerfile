FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Clientes/Clientes.API/Clientes.API.csproj", "Clientes/Clientes.API/"]
COPY ["Clientes/Clientes.Application/Clientes.Application.csproj", "Clientes/Clientes.Application/"]
COPY ["Clientes/Clientes.Domain/Clientes.Domain.csproj", "Clientes/Clientes.Domain/"]
COPY ["Clientes/Clientes.Infrastructure/Clientes.Infrastructure.csproj", "Clientes/Clientes.Infrastructure/"]
COPY ["Shared/Facturacion.Shared/Facturacion.Shared.csproj", "Shared/Facturacion.Shared/"]
RUN dotnet restore "Clientes/Clientes.API/Clientes.API.csproj"
COPY . .
WORKDIR /src/Clientes/Clientes.API

FROM build AS publish
WORKDIR /src/Clientes/Clientes.API
RUN dotnet publish "Clientes.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Clientes.API.dll"]
