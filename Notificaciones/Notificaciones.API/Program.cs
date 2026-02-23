using Microsoft.EntityFrameworkCore;
using Notificaciones.API.Workers;
using Notificaciones.Application.EventHandlers;
using Notificaciones.Domain.Interfaces;
using Notificaciones.Infrastructure.Data;
using Notificaciones.Infrastructure.Email;
using Notificaciones.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ── Controllers y Swagger ────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Notificaciones API", Version = "v1" });
});

// ── Base de datos ────────────────────────────────────────────────────────────
builder.Services.AddDbContext<NotificacionesDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.MigrationsAssembly(typeof(Program).Assembly.FullName)));

// ── Repositorios ────────────────────────────────────────────────────────────
builder.Services.AddScoped<INotificacionRepository, NotificacionRepository>();

// ── Email ────────────────────────────────────────────────────────────────────
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();

// ── Handlers de aplicación (Scoped para que el Worker los resuelva por scope)
builder.Services.AddScoped<FacturaTransmitidaDianHandler>();

// ── Worker de mensajería (Singleton: se inicia con la app) ──────────────────
builder.Services.AddHostedService<EventConsumerWorker>();

// ── CORS ─────────────────────────────────────────────────────────────────────
builder.Services.AddCors(opts =>
    opts.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

// ── Middleware ───────────────────────────────────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// ── Migración automática al iniciar ─────────────────────────────────────────
try
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<NotificacionesDbContext>();
    var logger  = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    logger.LogInformation("[Notificaciones] Aplicando migraciones...");
    await context.Database.MigrateAsync();
    logger.LogInformation("[Notificaciones] Migraciones aplicadas OK.");
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "[Notificaciones] Error aplicando migraciones.");
    if (app.Environment.IsProduction()) throw;
}

app.Run();
