using ClimaApi.DTOs;

namespace ClimaApi.Services;

public interface IClimaService
{
    ClimaRespuestaDto ObtenerRecomendacionClima(string ciudad);
}

public class ClimaService : IClimaService
{
    private static readonly string[] Condiciones = ["Soleado", "Lluvioso", "Nublado", "Frío", "Tormentoso"];

    public ClimaRespuestaDto ObtenerRecomendacionClima(string ciudad)
    {
        var condicion = Condiciones[Random.Shared.Next(Condiciones.Length)];
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

        return new ClimaRespuestaDto
        {
            Ciudad = ciudad,
            Temperatura = $"{temperatura}°C",
            Condicion = condicion,
            Recomendacion = recomendacion
        };
    }
}
