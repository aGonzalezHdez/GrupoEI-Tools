using Application.DTOs;

namespace Application.Interfaces.Repositories;

public interface IUsersRepository
{
    Task<OperationResult> ResetPassword(string user,string defaultPassword, CancellationToken ct);
    Task<OperationResult> DuplicaVentanaUsuarios(string usuarioOrigen, string usuarioDestino, CancellationToken ct);
    Task<OperationResult> ActualizaPuesto(string usuario, CancellationToken ct);
    Task<OperationResult> AgregregaPantallaMasivo(IEnumerable<string> usuarios,IEnumerable<int> pantallas, CancellationToken ct);
    Task<OperationResult> DeshabilitarUsuario(IEnumerable<string> usuarios, string motivoBaja,string usuarioBaja, CancellationToken ct);
    
}