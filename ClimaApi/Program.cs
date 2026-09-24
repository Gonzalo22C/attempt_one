using System.Diagnostics;
using ClimaApi.DTOs;
using ClimaApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IClimaService, ClimaService>();

var app = builder.Build();

var appStartTime = DateTime.UtcNow;

// Middleware global para captura y manejo de excepciones no controladas
app.Use(async (context, next) =>
{
    try
    {
        await next(context);
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Error no controlado capturado por el middleware global: {Message}", ex.Message);

        if (!context.Response.HasStarted)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var errorResponse = new
            {
                estado = StatusCodes.Status500InternalServerError,
                error = "Error interno del servidor",
                mensaje = "Ocurrió un error inesperado al procesar la solicitud.",
                detalle = ex.Message,
                fecha = DateTime.UtcNow
            };

            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
});

// Middleware personalizado para registrar método HTTP, ruta y tiempo de ejecución
app.Use(async (context, next) =>
{
    var stopwatch = Stopwatch.StartNew();
    await next(context);
    stopwatch.Stop();
    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {context.Request.Method} {context.Request.Path} - {stopwatch.ElapsedMilliseconds} ms");
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Endpoint 1: '/'
app.MapGet("/", () => Results.Ok(new
{
    mensaje = "API de Clima v1.1 - Refactorizada con DTOs y Servicios",
    estado = "Activo",
    endpoints = new[]
    {
        "/",
        "/health",
        "/api/clima/recomendacion/{ciudad}",
        "/swagger"
    }
}));

// Endpoint 2: '/health'
app.MapGet("/health", () =>
{
    var uptime = DateTime.UtcNow - appStartTime;
    var memoriaSimuladaMb = Math.Round(Random.Shared.NextDouble() * (128.0 - 45.0) + 45.0, 2);

    return Results.Ok(new
    {
        estado = "Saludable",
        consumoMemoriaSimulado = $"{memoriaSimuladaMb} MB",
        uptime = $"{uptime.Days}d {uptime.Hours:D2}h {uptime.Minutes:D2}m {uptime.Seconds:D2}s",
        totalSegundos = Math.Floor(uptime.TotalSeconds)
    });
})
.WithName("HealthCheck");

// Endpoint 3: '/api/clima/recomendacion/{ciudad}'
app.MapGet("/api/clima/recomendacion/{ciudad}", (string ciudad, IClimaService climaService) =>
{
    var resultado = climaService.ObtenerRecomendacionClima(ciudad);
    return Results.Ok(resultado);
})
.WithName("ObtenerRecomendacionClima");

app.Run();
