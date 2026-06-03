using System.Net.Http.Json;
using System.Text.Json;
using Application.Interfaces.Reporting;
using Infrastructure.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Gateways;

public class TokenProvider : ITokenProvider
{
    private readonly HttpClient _httpClient;
    private readonly TokenOptions _options;
    private readonly ILogger<TokenProvider> _logger;

    public TokenProvider(
        HttpClient httpClient,
        IOptions<TokenOptions> options,
        ILogger<TokenProvider> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string?> GetTokenAsync(CancellationToken ct)
    {
        var url = $"{_options.BaseUrl.TrimEnd('/')}/{_options.TokenPath.TrimStart('/')}";

        // El servicio de autenticación espera JSON con estas propiedades.
        var payload = new
        {
            username = _options.Username,
            passsEncryp = _options.PasssEncryp,
            idPantalla = _options.IdPantalla
        };

        try
        {
            using var response = await _httpClient.PostAsJsonAsync(url, payload, ct);
            var body = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Servicio de Token respondió {StatusCode}", (int)response.StatusCode);
                return null;
            }

            var token = ExtractToken(body);

            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogWarning("El servicio de Token no devolvió un token válido");
                return null;
            }

            return token;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            _logger.LogError(ex, "Error al obtener el Token en {Url}", url);
            return null;
        }
    }

    private static string? ExtractToken(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return null;

        try
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            // Caso 1: la respuesta es directamente el token como string.
            if (root.ValueKind == JsonValueKind.String)
                return root.GetString();

            // Caso 2: el token viene dentro de una propiedad conocida.
            if (root.ValueKind == JsonValueKind.Object)
            {
                foreach (var name in new[] { "token", "Token", "access_token", "accessToken" })
                {
                    if (root.TryGetProperty(name, out var prop) && prop.ValueKind == JsonValueKind.String)
                        return prop.GetString();
                }
            }

            return null;
        }
        catch (JsonException)
        {
            // La respuesta no es JSON: se asume que el cuerpo es el token crudo.
            return body.Trim().Trim('"');
        }
    }
}
