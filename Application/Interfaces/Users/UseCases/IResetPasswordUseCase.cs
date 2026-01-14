using Application.DTOs;

namespace Application.Interfaces.Users.UseCases;

public interface IResetPasswordUseCase
{
    Task<OperationResult> ResetPassword(string? user, CancellationToken ct);
}