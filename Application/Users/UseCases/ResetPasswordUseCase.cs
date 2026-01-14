using Application.Common.Security;
using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Users.UseCases;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Users.UseCases;

public class ResetPasswordUseCase : IResetPasswordUseCase
{
    private readonly ILogger<ResetPasswordUseCase> _logger;
    private readonly IUsersRepository _usersRepository;
    private readonly SecurityOptions _secutiryOptions;

    public ResetPasswordUseCase(ILogger<ResetPasswordUseCase> logger, IUsersRepository usersRepository, IOptions<SecurityOptions> securityOptions)
    {
        _logger = logger;
        _usersRepository = usersRepository;
        _secutiryOptions = securityOptions.Value;
    }
    
    public async Task<OperationResult> ResetPassword(string? user, CancellationToken ct)
    {
        return  await _usersRepository.ResetPassword(user, _secutiryOptions.TemporaryPassword ,ct);
    }
}