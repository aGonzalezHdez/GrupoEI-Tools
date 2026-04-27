using Application.DTOs;

namespace Application.Interfaces.Users.UseCases;

public interface IBajaUsuarioUseCase
{
    Task<OperationResult> DeshabilitarUsuario(IEnumerable<string> usuarios, string motivoBaja,string usuarioBaja, CancellationToken ct);
}