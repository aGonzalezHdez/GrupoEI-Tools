using System.Text.Json;
using Application.Common.Validator;
using Application.Interfaces;
using Application.Interfaces.Reporting.UseCase;
using Application.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Reporting.UseCase;

public class VerificacionComprobanteFiscalUseCase : IVerificacionComprobanteFiscalUseCase
{
    private readonly ILogger<VerificacionComprobanteFiscalUseCase> _logger;
    private readonly IVerificacionCfdiRepository _verificacionCfdiRepository;
    
    public async Task VerificacionCfdiAsync(string? uuid, CancellationToken ct)
    {
        //ejemplo uuid = "412ac445-f342-4a20-b50f-e73f885a1054";
        if (string.IsNullOrEmpty(uuid))
        {
            _logger.LogWarning("Se recibe uuid vacio");
            return;
        }

        if (!InputValidators.IsValidUuid(uuid))
        {
            Console.WriteLine("Se recibe uuid invalido");
            _logger.LogWarning("Se recibe uuid invalido");
            return;
        }
        
        var result = await _verificacionCfdiRepository.GetByUuidAsync(uuid, ct);

        if (result is null)
        {
            _logger.LogWarning("No existe información para UUID={UUID}", uuid);
            return;
        }
        
        //Se imprime como json en pantalla
        Console.WriteLine(
            JsonSerializer.Serialize(
                result,
                new JsonSerializerOptions { WriteIndented = true }));
        _logger.LogInformation("UUID={UUID} Estado={Estado} CodigoEstatus={CodigoEstatus}",
            result.Uuid, result.Estado, result.CodigoEstatus);
    }
}