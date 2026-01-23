FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Usuarios/Usuarios.API/Usuarios.API.csproj", "Usuarios/Usuarios.API/"]
COPY ["Usuarios/Usuarios.Application/Usuarios.Application.csproj", "Usuarios/Usuarios.Application/"]
COPY ["Usuarios/Usuarios.Domain/Usuarios.Domain.csproj", "Usuarios/Usuarios.Domain/"]
COPY ["Usuarios/Usuarios.Infrastructure/Usuarios.Infrastructure.csproj", "Usuarios/Usuarios.Infrastructure/"]
RUN dotnet restore "Usuarios/Usuarios.API/Usuarios.API.csproj"
COPY . .
WORKDIR /src/Usuarios/Usuarios.API

FROM build AS publish
WORKDIR /src/Usuarios/Usuarios.API
RUN dotnet publish "Usuarios.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Usuarios.API.dll"]
