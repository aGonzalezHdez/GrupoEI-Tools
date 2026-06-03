using Application.DTOs.Reporting;

namespace Application.Interfaces.Reporting.UseCase;

public interface ICargarControlUseCase
{
    Task<CargarControlResultado> CargarControlAsync(CargarControlRequest request, CancellationToken ct);
}
