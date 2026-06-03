namespace Infrastructure.Http;

public sealed class TokenOptions
{
    public string BaseUrl { get; init; } = string.Empty;
    public string TokenPath { get; init; } = "/api/Usuarios/Token";

    // Credenciales del servicio (se configuran por ambiente, no en código).
    public string Username { get; init; } = string.Empty;
    public string PasssEncryp { get; init; } = string.Empty;
    public int IdPantalla { get; init; }

    public int TimeoutSeconds { get; init; } = 30;
}
