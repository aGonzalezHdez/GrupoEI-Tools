using Application.DTOs;
using Application.Interfaces.Users;
using Application.Interfaces.Users.UseCases;

namespace Application.Users;

public class UsersUseCase : IUsersUseCase
{
    private readonly IDuplicaVentanaUsuarioUseCase _duplicaVentana;
    private readonly IResetPasswordUseCase _resetPassword;
    private readonly IActualizaPuestoUseCase _actualizaPuesto;

    public UsersUseCase(IDuplicaVentanaUsuarioUseCase duplicaVentana, IResetPasswordUseCase resetPassword, IActualizaPuestoUseCase actualizaPuesto)
    {
        _duplicaVentana = duplicaVentana;
        _resetPassword = resetPassword;
        _actualizaPuesto = actualizaPuesto;
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
}