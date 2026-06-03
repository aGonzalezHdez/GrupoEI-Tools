using Application.DTOs.Reporting;
using Application.Interfaces.Reporting;
using Application.Interfaces.Reporting.UseCase;

namespace Application.Reporting;

public class ReportingUseCase : IReportingUseCase
{
    private readonly IVerificacionComprobanteFiscalUseCase _verificacionCfdi;
    private readonly ICargarControlUseCase _cargarControl;

    public ReportingUseCase(
        IVerificacionComprobanteFiscalUseCase verificacionCfdi,
        ICargarControlUseCase cargarControl)
    {
        _verificacionCfdi = verificacionCfdi;
        _cargarControl = cargarControl;
    }

    public async Task VerificacionCfdiAsync(string? uuid, CancellationToken ct)
    {
        await _verificacionCfdi.VerificacionCfdiAsync(uuid, ct);
    }

    public async Task<CargarControlResultado> CargarControlAsync(CargarControlRequest request, CancellationToken ct)
    {
        return await _cargarControl.CargarControlAsync(request, ct);
    }
}
