using Application.DTOs;

namespace Application.Interfaces.Users.UseCases;

public interface IDuplicaVentanaUsuarioUseCase
{
    Task<OperationResult> DuplicaVentanaUsuario(string? usuarioOrigen, string? usuarioDestino, CancellationToken ct);
    Task<OperationResult> AgregregaPantallaMasivo(IEnumerable<string>? users, IEnumerable<int>? pantallas,CancellationToken ct);
}