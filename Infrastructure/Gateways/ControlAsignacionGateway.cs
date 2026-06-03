using System.Net.Http.Headers;
using System.Text.Json;
using Application.DTOs.Reporting;
using Application.Interfaces.Reporting;
using Infrastructure.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Gateways;

public class ControlAsignacionGateway : IControlAsignacionGateway
{
    private readonly HttpClient _httpClient;
    private readonly ControlAsignacionOptions _options;
    private readonly ITokenProvider _tokenProvider;
    private readonly ILogger<ControlAsignacionGateway> _logger;

    public ControlAsignacionGateway(
        HttpClient httpClient,
        IOptions<ControlAsignacionOptions> options,
        ITokenProvider tokenProvider,
        ILogger<ControlAsignacionGateway> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _tokenProvider = tokenProvider;
        _logger = logger;
    }

    public async Task<CargarControlResultado> CargarControlAsync(CargarControlRequest request, CancellationToken ct)
    {
        var url = $"{_options.BaseUrl.TrimEnd('/')}/{_options.CargarControlPath.TrimStart('/')}";

        // Se obtiene el bearer token antes de invocar el servicio.
        var token = await _tokenProvider.GetTokenAsync(ct);
        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogWarning("No se pudo obtener el bearer token; se cancela la llamada a CargarControl");
            return new CargarControlResultado
            {
                Success = false,
                StatusCode = 401,
                Mensaje = "No fue posible obtener el token de autenticación."
            };
        }

        var form = new Dictionary<string, string>
        {
            ["IdOficina"] = request.IdOficina.ToString(),
            ["Aduana"] = request.Aduana.ToString(),
            ["Operacion"] = request.Operacion.ToString(),
            ["idDepartamento"] = request.IdDepartamento.ToString()
        };

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new FormUrlEncodedContent(form)
        };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        _logger.LogInformation(
            "POST {Url} IdOficina={IdOficina} Aduana={Aduana} Operacion={Operacion} idDepartamento={IdDepartamento}",
            url, request.IdOficina, request.Aduana, request.Operacion, request.IdDepartamento);

        try
        {
            using var response = await _httpClient.SendAsync(httpRequest, ct);
            var body = await response.Content.ReadAsStringAsync(ct);

            var resultado = new CargarControlResultado
            {
                Success = response.IsSuccessStatusCode,
                StatusCode = (int)response.StatusCode
            };

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("CargarControl respondió {StatusCode}", (int)response.StatusCode);
                resultado.Mensaje = $"El servicio respondió {(int)response.StatusCode} ({response.ReasonPhrase}).";
                resultado.Datos = body;
                return resultado;
            }

            resultado.Datos = TryParseJson(body);
            return resultado;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            _logger.LogError(ex, "Error al invocar CargarControl en {Url}", url);
            return new CargarControlResultado
            {
                Success = false,
                StatusCode = 502,
                Mensaje = $"No fue posible contactar al servicio externo: {ex.Message}"
            };
        }
    }

    private static object? TryParseJson(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return null;

        try
        {
            using var doc = JsonDocument.Parse(body);
            return doc.RootElement.Clone();
        }
        catch (JsonException)
        {
            return body;
        }
    }
}
