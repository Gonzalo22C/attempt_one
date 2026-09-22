using System.Diagnostics;
using ClimaApi.DTOs;
using ClimaApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddScoped<IClimaService, ClimaService>();

var app = builder.Build();

var appStartTime = DateTime.UtcNow;

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
    app.MapOpenApi();
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
        "/api/clima/recomendacion/{ciudad}"
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
