using Application.DTOs;
using Application.Interfaces.Users;
using Application.Interfaces.Users.UseCases;

namespace Application.Users;

public class UsersUseCase : IUsersUseCase
{
    private readonly IDuplicaVentanaUsuarioUseCase _duplicaVentana;
    private readonly IResetPasswordUseCase _resetPassword;
    private readonly IActualizaPuestoUseCase _actualizaPuesto;
    private readonly IBajaUsuarioUseCase _bajaUsuario;

    public UsersUseCase(IDuplicaVentanaUsuarioUseCase duplicaVentana, IResetPasswordUseCase resetPassword, IActualizaPuestoUseCase actualizaPuesto,IBajaUsuarioUseCase bajaUsuario)
    {
        _duplicaVentana = duplicaVentana;
        _resetPassword = resetPassword;
        _actualizaPuesto = actualizaPuesto;
        _bajaUsuario = bajaUsuario;
    }
    public async Task<OperationResult> DuplicaVentanaUsuario(string? usuarioOrigen, string? usuarioDestino, CancellationToken ct)
    {
        return await _duplicaVentana.DuplicaVentanaUsuario(usuarioOrigen,usuarioDestino, ct);
    }
    public async Task<OperationResult> ResetPassword(string? user, CancellationToken ct)
    {
        return await _resetPassword.ResetPassword(user, ct);
    }

    public async Task<OperationResult> ActualizaPuesto(string? user, CancellationToken ct)
    {
        return (await _actualizaPuesto.ActualizaPuesto(user, ct));
    }

    public async Task<OperationResult> AgregregaPantallaMasivo(IEnumerable<string>? users, IEnumerable<int> pantallas, CancellationToken ct)
    {
        return (await _duplicaVentana.AgregregaPantallaMasivo(users, pantallas, ct));
    }

    public async Task<OperationResult> DeshabilitarUsuario(IEnumerable<string> usuarios, string motivoBaja,string usuarioBaja, CancellationToken ct)
    {
        return (await _bajaUsuario.DeshabilitarUsuario(usuarios, motivoBaja,usuarioBaja, ct));
    }
}