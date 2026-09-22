namespace ClimaApi.DTOs;

public class ClimaRespuestaDto
{
    public string Ciudad { get; set; } = string.Empty;
    public string Temperatura { get; set; } = string.Empty;
    public string Condicion { get; set; } = string.Empty;
    public string Recomendacion { get; set; } = string.Empty;
}
