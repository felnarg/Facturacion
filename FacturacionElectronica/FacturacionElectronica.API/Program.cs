using FacturacionElectronica.Infrastructure.Data;
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
    
    logger.LogInformation("Applying database migrations...");
    
    // Aplicar migraciones
    await context.Database.MigrateAsync();
    
    logger.LogInformation("Database migrations applied successfully.");
    
    // Ejecutar seed de datos
    logger.LogInformation("Seeding database...");
    await FacturacionElectronica.Infrastructure.Data.SeedData.Initialize(context);
    logger.LogInformation("Database seeded successfully.");
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    
    // En producción, podríamos querer salir si la base de datos no está lista
    if (app.Environment.IsProduction())
    {
        throw;
    }
}

app.Run();