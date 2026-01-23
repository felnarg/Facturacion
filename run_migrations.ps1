#!/usr/bin/env powershell

$services = @("Catalogo", "Clientes", "Compras", "Inventario", "Usuarios", "Ventas")
$basePath = "c:\Users\Usuario\source\repos\Facturacion"

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  Ejecutando Migraciones" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Cyan

foreach ($service in $services) {
    $apiPath = "$basePath\$service\$service.API"
    
    Write-Host "`nEjecutando: $service.API" -ForegroundColor Yellow
    Write-Host "Ruta: $apiPath" -ForegroundColor Gray
    
    $process = Start-Process -FilePath "dotnet" `
        -ArgumentList "run --no-build" `
        -WorkingDirectory $apiPath `
        -PassThru `
        -NoNewWindow
    
    # Esperar 12 segundos para las migraciones
    Start-Sleep -Seconds 12
    
    # Matar el proceso si sigue ejecutándose
    if ($process -and -not $process.HasExited) {
        Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
        Start-Sleep -Seconds 1
    }
    
    Write-Host "OK: $service - Migraciones completadas" -ForegroundColor Green
}

Write-Host "`n=====================================" -ForegroundColor Cyan
Write-Host "  Todas las Migraciones Completadas" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Cyan
