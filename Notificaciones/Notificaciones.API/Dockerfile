FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de proyecto
COPY ["Notificaciones/Notificaciones.API/Notificaciones.API.csproj",            "Notificaciones/Notificaciones.API/"]
COPY ["Notificaciones/Notificaciones.Application/Notificaciones.Application.csproj", "Notificaciones/Notificaciones.Application/"]
COPY ["Notificaciones/Notificaciones.Domain/Notificaciones.Domain.csproj",       "Notificaciones/Notificaciones.Domain/"]
COPY ["Notificaciones/Notificaciones.Infrastructure/Notificaciones.Infrastructure.csproj", "Notificaciones/Notificaciones.Infrastructure/"]
COPY ["Shared/Facturacion.Shared/Facturacion.Shared.csproj",                    "Shared/Facturacion.Shared/"]

RUN dotnet restore "Notificaciones/Notificaciones.API/Notificaciones.API.csproj"

# Copiar código fuente
COPY . .

WORKDIR "/src/Notificaciones/Notificaciones.API"
RUN dotnet build "Notificaciones.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Notificaciones.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notificaciones.API.dll"]
