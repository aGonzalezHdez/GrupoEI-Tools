namespace Application.Interfaces.Manifiestos.Imports;

public sealed record PatchApplyResult(bool Found, bool Changed, List<string> Messages);

public interface ICorrectivePatchStore
{
    Task<bool> ExistsAsync(string guiaHouse, CancellationToken ct);

    Task<PatchApplyResult> ApplyPatchAsync(
        string guiaHouse,
        Dictionary<string, object?> patch,
        CancellationToken ct);
}