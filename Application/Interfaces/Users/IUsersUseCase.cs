using Application.DTOs;

namespace Application.Interfaces.Users;

public interface IUsersUseCase
{
    Task<OperationResult> DuplicaVentanaUsuario(string? usuarioOrigen, string? usuarioDestino, CancellationToken ct);
    Task<OperationResult> ResetPassword(string? user, CancellationToken ct);
    Task<OperationResult> ActualizaPuesto(string? user, CancellationToken ct);

}