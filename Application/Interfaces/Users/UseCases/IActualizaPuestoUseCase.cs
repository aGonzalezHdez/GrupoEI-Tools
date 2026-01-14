using Application.DTOs;

namespace Application.Interfaces.Users.UseCases;

public interface IActualizaPuestoUseCase
{
    Task<OperationResult> ActualizaPuesto(string? user, CancellationToken ct);
}