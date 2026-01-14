using Application.DTOs;

namespace Application.Interfaces.Users.UseCases;

public interface IDuplicaVentanaUsuarioUseCase
{
    Task<OperationResult> DuplicaVentanaUsuario(string? usuarioOrigen, string? usuarioDestino, CancellationToken ct);
}