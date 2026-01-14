using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Users.UseCases;
using Microsoft.Extensions.Logging;

namespace Application.Users.UseCases;

public class ActualizaPuestoUseCase : IActualizaPuestoUseCase
{
    private readonly ILogger<DuplicaVentanaUsuarioUseCase> _logger;
    private readonly IUsersRepository _usersRepository;

    public ActualizaPuestoUseCase(ILogger<DuplicaVentanaUsuarioUseCase> logger, IUsersRepository usersRepository)
    {
        _logger = logger;
        _usersRepository = usersRepository;
    }

    public async Task<OperationResult> ActualizaPuesto(string? user,CancellationToken ct)
    {
        _logger.LogInformation("Iniciando Actualizacion de puesto");
        return  await _usersRepository.ActualizaPuesto(user, ct);
    }
}