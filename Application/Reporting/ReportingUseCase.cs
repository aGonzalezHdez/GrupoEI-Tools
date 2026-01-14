using Application.Interfaces.Reporting;
using Application.Interfaces.Reporting.UseCase;

namespace Application.Reporting;

public class ReportingUseCase : IReportingUseCase
{
    private readonly IVerificacionComprobanteFiscalUseCase _verificacionCfdi;

    public ReportingUseCase(IVerificacionComprobanteFiscalUseCase verificacionCfdi)
    {
        _verificacionCfdi = verificacionCfdi;
    }
    public async Task VerificacionCfdiAsync(string? uuid, CancellationToken ct)
    {
        await _verificacionCfdi.VerificacionCfdiAsync(uuid, ct);
    }
}