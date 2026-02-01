FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Kardex/Kardex.API/Kardex.API.csproj", "Kardex/Kardex.API/"]
COPY ["Kardex/Kardex.Application/Kardex.Application.csproj", "Kardex/Kardex.Application/"]
COPY ["Kardex/Kardex.Domain/Kardex.Domain.csproj", "Kardex/Kardex.Domain/"]
COPY ["Kardex/Kardex.Infrastructure/Kardex.Infrastructure.csproj", "Kardex/Kardex.Infrastructure/"]
COPY ["Shared/Facturacion.Shared/Facturacion.Shared.csproj", "Shared/Facturacion.Shared/"]
RUN dotnet restore "Kardex/Kardex.API/Kardex.API.csproj"
COPY . .
WORKDIR /src/Kardex/Kardex.API

FROM build AS publish
WORKDIR /src/Kardex/Kardex.API
RUN dotnet publish "Kardex.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Kardex.API.dll"]
