namespace Application.Interfaces.Reporting;

public interface IReportingUseCase
{
    Task VerificacionCfdiAsync(string? uuid, CancellationToken ct);
}