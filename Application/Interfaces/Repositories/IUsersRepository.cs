using Application.DTOs;

namespace Application.Interfaces.Repositories;

public interface IUsersRepository
{
    Task<OperationResult> ResetPassword(string user,string defaultPassword, CancellationToken ct);
    Task<OperationResult> DuplicaVentanaUsuarios(string usuarioOrigen, string usuarioDestino, CancellationToken ct);
    Task<OperationResult> ActualizaPuesto(string usuario, CancellationToken ct);
}