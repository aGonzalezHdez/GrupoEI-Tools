using Application.DTOs;

namespace Application.Interfaces.Repositories;

public interface IVerificacionCfdiRepository
{
    Task<VerificacionCfdiDto?> GetByUuidAsync(string uuid, CancellationToken ct);
}