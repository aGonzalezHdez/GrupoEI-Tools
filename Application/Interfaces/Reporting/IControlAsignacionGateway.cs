using Application.DTOs.Reporting;

namespace Application.Interfaces.Reporting;

public interface IControlAsignacionGateway
{
    Task<CargarControlResultado> CargarControlAsync(CargarControlRequest request, CancellationToken ct);
}
