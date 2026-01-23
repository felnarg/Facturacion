#!/usr/bin/env powershell

# Script para iniciar todos los microservicios y el API Gateway

$basePath = "c:\Users\Usuario\source\repos\Facturacion"
$services = @(
    @{ name = "Catalogo"; path = "Catalogo\Catalogo.API"; port = "5008" },
    @{ name = "Clientes"; path = "Clientes\Clientes.API"; port = "5013" },
    @{ name = "Compras"; path = "Compras\Compras.API"; port = "5011" },
    @{ name = "Inventario"; path = "Inventario\Inventario.API"; port = "5010" },
    @{ name = "Usuarios"; path = "Usuarios\Usuarios.API"; port = "5014" },
    @{ name = "Ventas"; path = "Ventas\Ventas.API"; port = "5012" },
    @{ name = "ApiGateway"; path = "ApiGateway\ApiGateway"; port = "5015" }
)

Write-Host "==============================================" -ForegroundColor Cyan
Write-Host "  Iniciando Microservicios y API Gateway" -ForegroundColor Green
Write-Host "==============================================" -ForegroundColor Cyan
Write-Host ""

$jobs = @()

foreach ($service in $services) {
    $fullPath = Join-Path $basePath $service.path
    $name = $service.name
    $port = $service.port
    
    Write-Host "Iniciando $name en puerto $port..." -ForegroundColor Yellow
    
    $job = Start-Process -FilePath "dotnet" `
        -ArgumentList "run --no-build" `
        -WorkingDirectory $fullPath `
        -PassThru `
        -NoNewWindow `
        -RedirectStandardOutput "$basePath\logs\$name.log" `
        -RedirectStandardError "$basePath\logs\$name.err"
    
    $jobs += @{ name = $name; port = $port; job = $job; path = $fullPath }
    
    Write-Host "✓ $name iniciado (PID: $($job.Id))" -ForegroundColor Green
    Start-Sleep -Seconds 2
}

Write-Host ""
Write-Host "==============================================" -ForegroundColor Green
Write-Host "  Todos los servicios iniciados" -ForegroundColor Cyan
Write-Host "==============================================" -ForegroundColor Green
Write-Host ""

Write-Host "Servicios en ejecución:" -ForegroundColor Cyan
foreach ($service in $jobs) {
    $status = if ($service.job.HasExited) { "DETENIDO" } else { "EJECUTÁNDOSE" }
    Write-Host "  - $($service.name) (Puerto: $($service.port)) - $status" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Acceda a los servicios en:" -ForegroundColor Cyan
Write-Host "  - API Gateway:    http://localhost:5015" -ForegroundColor White
Write-Host "  - Catalogo:       http://localhost:5008/swagger" -ForegroundColor White
Write-Host "  - Clientes:       http://localhost:5013/swagger" -ForegroundColor White
Write-Host "  - Compras:        http://localhost:5011/swagger" -ForegroundColor White
Write-Host "  - Inventario:     http://localhost:5010/swagger" -ForegroundColor White
Write-Host "  - Usuarios:       http://localhost:5014/swagger" -ForegroundColor White
Write-Host "  - Ventas:         http://localhost:5012/swagger" -ForegroundColor White
Write-Host ""
Write-Host "Presione Ctrl+C para detener todos los servicios" -ForegroundColor Gray
Write-Host ""

# Mantener el script ejecutándose y monitorear los servicios
try {
    while ($true) {
        Start-Sleep -Seconds 10
        
        # Verificar si algún servicio se ha detenido
        foreach ($service in $jobs) {
            if ($service.job.HasExited) {
                Write-Host "ADVERTENCIA: $($service.name) se ha detenido (Código: $($service.job.ExitCode))" -ForegroundColor Red
            }
        }
    }
}
catch {
    Write-Host "Deteniendo servicios..." -ForegroundColor Yellow
    foreach ($service in $jobs) {
        if ($service.job -and -not $service.job.HasExited) {
            Stop-Process -Id $service.job.Id -Force -ErrorAction SilentlyContinue
        }
    }
    Write-Host "Servicios detenidos" -ForegroundColor Green
}
