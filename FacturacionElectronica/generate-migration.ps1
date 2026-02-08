# Script para generar migraciones de Entity Framework

Write-Host "=== Generando Migración para Facturación Electrónica ===" -ForegroundColor Cyan

# Restaurar paquetes NuGet
Write-Host "Restaurando paquetes NuGet..." -ForegroundColor Yellow
dotnet restore

# Generar migración
Write-Host "Generando migración inicial..." -ForegroundColor Yellow
dotnet ef migrations add InitialCreate `
    --project "FacturacionElectronica.API/FacturacionElectronica.API.csproj" `
    --startup-project "FacturacionElectronica.API/FacturacionElectronica.API.csproj" `
    --context ApplicationDbContext `
    --output-dir "Migrations" `
    --verbose

# Verificar si la migración se generó correctamente
if ($LASTEXITCODE -eq 0) {
    Write-Host "`n✅ Migración generada exitosamente!" -ForegroundColor Green
    Write-Host "Ubicación: FacturacionElectronica.API/Migrations/" -ForegroundColor Green
    
    # Listar archivos de migración generados
    Write-Host "`nArchivos generados:" -ForegroundColor Cyan
    Get-ChildItem -Path "FacturacionElectronica.API/Migrations" -Filter "*.cs" | ForEach-Object {
        Write-Host "  - $($_.Name)" -ForegroundColor White
    }
} else {
    Write-Host "`n❌ Error al generar la migración" -ForegroundColor Red
    exit 1
}

Write-Host "`n=== Script completado ===" -ForegroundColor Cyan