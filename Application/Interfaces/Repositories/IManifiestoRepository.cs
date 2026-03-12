using Application.DTOs;

namespace Application.Interfaces.Repositories;

public interface IManifiestoRepository
{
    Task<OperationResult> UpdateManifiestos(List<string> manifiestos, CancellationToken ct);
}