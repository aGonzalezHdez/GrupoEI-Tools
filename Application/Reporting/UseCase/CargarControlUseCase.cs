using Application.DTOs.Reporting;
using Application.Interfaces.Reporting;
using Application.Interfaces.Reporting.UseCase;
using Microsoft.Extensions.Logging;

namespace Application.Reporting.UseCase;

public class CargarControlUseCase : ICargarControlUseCase
{
    private readonly IControlAsignacionGateway _gateway;
    private readonly ILogger<CargarControlUseCase> _logger;

    public CargarControlUseCase(IControlAsignacionGateway gateway, ILogger<CargarControlUseCase> logger)
    {
        _gateway = gateway;
        _logger = logger;
    }

    public async Task<CargarControlResultado> CargarControlAsync(CargarControlRequest request, CancellationToken ct)
    {
        if (request is null)
        {
            _logger.LogWarning("Se recibe request nulo en CargarControl");
            return new CargarControlResultado
            {
                Success = false,
                StatusCode = 400,
                Mensaje = "La solicitud no puede ser nula."
            };
        }

        _logger.LogInformation(
            "CargarControl IdOficina={IdOficina} Aduana={Aduana} Operacion={Operacion} idDepartamento={IdDepartamento}",
            request.IdOficina, request.Aduana, request.Operacion, request.IdDepartamento);

        return await _gateway.CargarControlAsync(request, ct);
    }
}
