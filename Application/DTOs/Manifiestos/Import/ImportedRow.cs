namespace Application.DTOs.Manifiestos.Import;

public sealed class ImportedRow
{
    public required int LineNumber { get; init; }
    public required Dictionary<string, object?> Values { get; init; }

    public string? GetString(string key) => Values.TryGetValue(key, out var v) ? v?.ToString() : null;
    public object? GetRaw(string key) => Values.TryGetValue(key, out var v) ? v : null;
}