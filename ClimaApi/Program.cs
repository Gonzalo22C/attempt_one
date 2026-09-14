var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Endpoint 1: '/'
app.MapGet("/", () => Results.Ok(new
{
    mensaje = "Bienvenido a la API de Clima",
    estado = "Activo",
    endpoints = new[]
    {
        "/",
        "/api/clima/recomendacion/{ciudad}"
    }
}));

// Endpoint 2: '/api/clima/recomendacion/{ciudad}'
app.MapGet("/api/clima/recomendacion/{ciudad}", (string ciudad) =>
{
    var condiciones = new[] { "Soleado", "Lluvioso", "Nublado", "Frío", "Tormentoso" };
    var condicion = condiciones[Random.Shared.Next(condiciones.Length)];
    var temperatura = Random.Shared.Next(5, 35);

    var recomendacion = condicion switch
    {
        "Soleado" => "Día soleado: usa protector solar, gafas de sol y mantente hidratado.",
        "Lluvioso" => "Día lluvioso: lleva paraguas o impermeable y calzado adecuado.",
        "Nublado" => "Cielo cubierto: buen clima para actividades al aire libre con abrigo ligero.",
        "Frío" => "Temperaturas bajas: viste ropa abrigada y consume bebidas calientes.",
        "Tormentoso" => "Alerta de tormenta: mantente resguardado y evita traslados innecesarios.",
        _ => "Disfruta de tu día."
    };

    return Results.Ok(new
    {
        ciudad,
        temperatura = $"{temperatura}°C",
        condicion,
        recomendacion,
        fecha = DateTime.UtcNow
    });
})
.WithName("ObtenerRecomendacionClima");

app.Run();
