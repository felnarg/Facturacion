using FacturacionElectronica.Domain.Interfaces;
using FacturacionElectronica.Infrastructure.Data;
using FacturacionElectronica.Infrastructure.EventBus;
using FacturacionElectronica.Infrastructure.Repositories;
using FacturacionElectronica.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext con configuración de migraciones en el proyecto API
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.MigrationsAssembly(typeof(Program).Assembly.FullName)));

// Registrar repositorios
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IDocumentoElectronicoRepository, DocumentoElectronicoRepository>();
builder.Services.AddScoped<IEmisorRepository, EmisorRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<INumeracionDocumentoRepository, NumeracionDocumentoRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Servicios de Facturación Electrónica DIAN
builder.Services.AddScoped<IXmlGeneratorService, XmlGeneratorService>();
builder.Services.AddScoped<IFirmaDigitalService, FirmaDigitalService>();
builder.Services.AddScoped<IDianSoapService, DianSoapService>();

// HttpClient para DIAN con timeout configurable
builder.Services.AddHttpClient("DianSoap", client =>
{
    var timeout = builder.Configuration.GetValue<int>("Dian:TimeoutSegundos", 120);
    client.Timeout = TimeSpan.FromSeconds(timeout);
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    // En desarrollo permite certificados autofirmados
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});

// EventBus (RabbitMQ) para publicar eventos a Notificaciones
builder.Services.AddSingleton<IEventBus>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new RabbitMQEventBus(
        hostName:     config["RabbitMQ:HostName"]  ?? "rabbitmq",
        userName:     config["RabbitMQ:UserName"]  ?? "admin",
        password:     config["RabbitMQ:Password"]  ?? "p@ssword123",
        exchangeName: config["RabbitMQ:Exchange"]  ?? "facturacion.events",
        queueName:    config["RabbitMQ:Queue"]     ?? "facturacion.electronica.events"
    );
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Ejecutar migraciones y seed AL INICIO de la aplicación
try
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    
    var context = services.GetRequiredService<ApplicationDbContext>();
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    logger.LogInformation("=== INICIANDO MIGRACIONES Y SEED ===");
    logger.LogInformation("Applying database migrations...");
    
    // Aplicar migraciones
    var migracionesPendientes = await context.Database.GetPendingMigrationsAsync();
    if (migracionesPendientes.Any())
    {
        logger.LogInformation($"Aplicando {migracionesPendientes.Count()} migraciones pendientes...");
        await context.Database.MigrateAsync();
        logger.LogInformation("Database migrations applied successfully.");
    }
    else
    {
        logger.LogInformation("No hay migraciones pendientes.");
    }
    
    // Verificar conexión
    var canConnect = await context.Database.CanConnectAsync();
    logger.LogInformation($"Conexión a base de datos: {(canConnect ? "OK" : "FALLO")}");
    
    // Verificar si hay datos
    var tieneEmisores = await context.Emisores.AnyAsync();
    var tieneClientes = await context.Clientes.AnyAsync();
    var tieneNumeraciones = await context.NumeracionesDocumentos.AnyAsync();
    
    logger.LogInformation($"Estado actual de la BD:");
    logger.LogInformation($"  - Emisores: {await context.Emisores.CountAsync()}");
    logger.LogInformation($"  - Clientes: {await context.Clientes.CountAsync()}");
    logger.LogInformation($"  - Numeraciones: {await context.NumeracionesDocumentos.CountAsync()}");
    
    // Ejecutar seed de datos
    logger.LogInformation("=== INICIANDO SEED DE DATOS ===");
    await FacturacionElectronica.Infrastructure.Data.SeedData.Initialize(context, logger);
    logger.LogInformation("=== SEED COMPLETADO ===");
    
    // Verificar datos después del seed
    logger.LogInformation($"Estado después del seed:");
    logger.LogInformation($"  - Emisores: {await context.Emisores.CountAsync()}");
    logger.LogInformation($"  - Clientes: {await context.Clientes.CountAsync()}");
    logger.LogInformation($"  - Numeraciones: {await context.NumeracionesDocumentos.CountAsync()}");
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "ERROR CRÍTICO: An error occurred while migrating or seeding the database.");
    logger.LogError($"Mensaje: {ex.Message}");
    logger.LogError($"Stack Trace: {ex.StackTrace}");
    if (ex.InnerException != null)
    {
        logger.LogError($"Inner Exception: {ex.InnerException.Message}");
    }
    
    // En producción, podríamos querer salir si la base de datos no está lista
    if (app.Environment.IsProduction())
    {
        throw;
    }
}

app.Run();