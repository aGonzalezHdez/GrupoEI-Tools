using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Users.UseCases;
using Microsoft.Extensions.Logging;

namespace Application.Users.UseCases;

public class DuplicaVentanaUsuarioUseCase : IDuplicaVentanaUsuarioUseCase
{
    private readonly ILogger<DuplicaVentanaUsuarioUseCase> _logger;
    private readonly IUsersRepository _usersRepository;

    public DuplicaVentanaUsuarioUseCase(ILogger<DuplicaVentanaUsuarioUseCase> logger, IUsersRepository usersRepository)
    {
        _logger = logger;
        _usersRepository = usersRepository;
    }
    
    public async Task<OperationResult> DuplicaVentanaUsuario(string? usuarioOrigen, string? usuarioDestino, CancellationToken ct)
    {
        return await _usersRepository.DuplicaVentanaUsuarios(usuarioOrigen, usuarioDestino, ct);
    }
}