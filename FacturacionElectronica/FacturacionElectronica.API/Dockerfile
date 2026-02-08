# Etapa de construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de proyecto y restaurar dependencias
COPY ["FacturacionElectronica.API/FacturacionElectronica.API.csproj", "FacturacionElectronica.API/"]
COPY ["FacturacionElectronica.Application/FacturacionElectronica.Application.csproj", "FacturacionElectronica.Application/"]
COPY ["FacturacionElectronica.Domain/FacturacionElectronica.Domain.csproj", "FacturacionElectronica.Domain/"]
COPY ["FacturacionElectronica.Infrastructure/FacturacionElectronica.Infrastructure.csproj", "FacturacionElectronica.Infrastructure/"]

RUN dotnet restore "FacturacionElectronica.API/FacturacionElectronica.API.csproj"

# Copiar todo el código fuente
COPY . .

# Publicar la aplicación
WORKDIR "/src/FacturacionElectronica.API"
RUN dotnet publish "FacturacionElectronica.API.csproj" -c Release -o /app/publish

# Etapa de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Instalar herramientas de diagnóstico
RUN apt-get update && apt-get install -y curl

COPY --from=build /app/publish .

# Crear usuario no root para mayor seguridad
RUN adduser --disabled-password --home /app --gecos '' appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "FacturacionElectronica.API.dll"]