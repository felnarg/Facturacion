FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Catalogo/Catalogo.API/Catalogo.API.csproj", "Catalogo/Catalogo.API/"]
COPY ["Catalogo/Catalogo.Application/Catalogo.Application.csproj", "Catalogo/Catalogo.Application/"]
COPY ["Catalogo/Catalogo.Domain/Catalogo.Domain.csproj", "Catalogo/Catalogo.Domain/"]
COPY ["Catalogo/Catalogo.Infrastructure/Catalogo.Infrastructure.csproj", "Catalogo/Catalogo.Infrastructure/"]
COPY ["Shared/Facturacion.Shared/Facturacion.Shared.csproj", "Shared/Facturacion.Shared/"]
RUN dotnet restore "Catalogo/Catalogo.API/Catalogo.API.csproj"
COPY . .
WORKDIR /src/Catalogo/Catalogo.API

FROM build AS publish
WORKDIR /src/Catalogo/Catalogo.API
RUN dotnet publish "Catalogo.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Catalogo.API.dll"]
