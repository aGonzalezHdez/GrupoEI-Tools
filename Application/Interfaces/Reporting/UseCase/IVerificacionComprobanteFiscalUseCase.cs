namespace Application.Interfaces.Reporting.UseCase;

public interface IVerificacionComprobanteFiscalUseCase
{
    Task VerificacionCfdiAsync(string? uuid, CancellationToken ct);
}