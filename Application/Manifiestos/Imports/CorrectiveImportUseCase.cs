using Application.DTOs.Manifiestos.Import;
using Application.Interfaces.Manifiestos.Imports;

namespace Application.Manifiestos.Imports;

public sealed class CorrectiveImportUseCase
{
    private readonly IReadOnlyDictionary<ImportFileType, IImportFileParser> _parsers;
    private readonly ICorrectivePatchStore _store;

    public CorrectiveImportUseCase(IEnumerable<IImportFileParser> parsers, ICorrectivePatchStore store)
    {
        _parsers = parsers.ToDictionary(p => p.FileType);
        _store = store;
    }

    public async Task<ImportReport> ExecuteAsync(CorrectiveImportRequest request, CancellationToken ct)
    {
        var started = DateTime.UtcNow;
        var lines = new List<ImportLineResult>();

        byte[] bytes;
        try { bytes = Convert.FromBase64String(request.ContentBase64); }
        catch { return Fatal(started, "Base64 inválido (no se pudo decodificar)."); }

        await using var ms = new MemoryStream(bytes);

        if (!_parsers.TryGetValue(request.FileType, out var parser))
            return Fatal(started, $"No hay parser registrado para FileType={request.FileType}.");

        var rows = await parser.ParseAsync(ms, request, ct);
        var selectedColumns = ResolveSelectedColumns(request.ColumnsToUpdate);

        int updated = 0, skipped = 0, warnings = 0, errors = 0;

        foreach (var row in rows)
        {
            var guiaHouse = row.GetString("GuiaHouse");

            if (string.IsNullOrWhiteSpace(guiaHouse))
            {
                errors++;
                lines.Add(new ImportLineResult(row.LineNumber, null, ImportLineStatus.Error,
                    new() { "GuiaHouse es requerido." }));
                continue;
            }

            var patch = BuildPatch(row, selectedColumns, request.OverwriteEmpty);

            if (patch.Count == 0)
            {
                skipped++;
                lines.Add(new ImportLineResult(row.LineNumber, guiaHouse, ImportLineStatus.Skipped,
                    new() { "Sin cambios (no hay valores aplicables en columnas seleccionadas)." }));
                continue;
            }

            var exists = await _store.ExistsAsync(guiaHouse, ct);
            if (!exists)
            {
                warnings++;
                lines.Add(new ImportLineResult(row.LineNumber, guiaHouse, ImportLineStatus.Warning,
                    new() { "GuiaHouse no existe en BD (proceso correctivo: no inserta)." }));
                continue;
            }

            if (request.DryRun)
            {
                updated++;
                lines.Add(new ImportLineResult(row.LineNumber, guiaHouse, ImportLineStatus.Updated,
                    new() { $"DryRun: se aplicarían {patch.Count} campos." }));
                continue;
            }

            var result = await _store.ApplyPatchAsync(guiaHouse, patch, ct);

            if (!result.Found)
            {
                warnings++;
                lines.Add(new ImportLineResult(row.LineNumber, guiaHouse, ImportLineStatus.Warning,
                    new() { "GuiaHouse no existe (cambió durante el proceso)." }));
                continue;
            }

            if (!result.Changed)
            {
                skipped++;
                lines.Add(new ImportLineResult(row.LineNumber, guiaHouse, ImportLineStatus.Skipped,
                    result.Messages.Count > 0 ? result.Messages : new() { "Sin cambios." }));
                continue;
            }

            updated++;
            lines.Add(new ImportLineResult(row.LineNumber, guiaHouse, ImportLineStatus.Updated,
                result.Messages.Count > 0 ? result.Messages : new() { "Actualizado." }));
        }

        var finished = DateTime.UtcNow;
        return new ImportReport(started, finished, new ImportSummary(rows.Count, updated, skipped, warnings, errors), lines);
    }

    private static HashSet<string> ResolveSelectedColumns(List<string>? columnsToUpdate)
    {
        if (columnsToUpdate is null || columnsToUpdate.Count == 0)
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase); // vacío => modo "todas" en BuildPatch

        if (columnsToUpdate.Any(c => c.Equals("GuiaHouse", StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("No se permite actualizar GuiaHouse (es la llave).");

        return columnsToUpdate.ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private static Dictionary<string, object?> BuildPatch(ImportedRow row, HashSet<string> selectedColumns, bool overwriteEmpty)
    {
        var patch = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        foreach (var kvp in row.Values)
        {
            var key = kvp.Key;

            if (key.Equals("GuiaHouse", StringComparison.OrdinalIgnoreCase))
                continue;

            if (selectedColumns.Count > 0 && !selectedColumns.Contains(key))
                continue;

            var value = kvp.Value;
            if (value is null) continue;

            if (!overwriteEmpty && value is string s && string.IsNullOrWhiteSpace(s))
                continue;

            patch[key] = value is string ss ? NormalizeText(ss) : value;
        }

        return patch;
    }

    private static string NormalizeText(string s)
        => string.Join(" ", s.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries));

    private static ImportReport Fatal(DateTime started, string message)
        => new(started, DateTime.UtcNow, new ImportSummary(0, 0, 0, 0, 1),
            new List<ImportLineResult> { new(0, null, ImportLineStatus.Error, new() { message }) });
}