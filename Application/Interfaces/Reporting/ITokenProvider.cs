namespace Application.Interfaces.Reporting;

public interface ITokenProvider
{
    /// <summary>
    /// Obtiene un bearer token del servicio de autenticación.
    /// Devuelve el token o null si no fue posible obtenerlo.
    /// </summary>
    Task<string?> GetTokenAsync(CancellationToken ct);
}
