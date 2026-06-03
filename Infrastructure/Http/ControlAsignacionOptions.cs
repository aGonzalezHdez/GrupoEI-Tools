namespace Infrastructure.Http;

public sealed class ControlAsignacionOptions
{
    public string BaseUrl { get; init; } = string.Empty;
    public string CargarControlPath { get; init; } = "/Reportes/ControldeAsignacion/CargarControl";
    public int TimeoutSeconds { get; init; } = 60;
}
