using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Users.UseCases;
using Microsoft.Extensions.Logging;

namespace Application.Users.UseCases;

public class BajaUsuarioUseCase : IBajaUsuarioUseCase
{
    private readonly ILogger<DuplicaVentanaUsuarioUseCase> _logger;
    private readonly IUsersRepository _usersRepository;

    public BajaUsuarioUseCase(ILogger<DuplicaVentanaUsuarioUseCase> logger,IUsersRepository usersRepository)
    {
        _logger = logger;
        _usersRepository = usersRepository;
    }
    public Task<OperationResult> DeshabilitarUsuario(IEnumerable<string> usuarios, string motivoBaja,string usuarioBaja, CancellationToken ct)
    {
        return (_usersRepository.DeshabilitarUsuario(usuarios, motivoBaja,usuarioBaja, ct));
    }
}