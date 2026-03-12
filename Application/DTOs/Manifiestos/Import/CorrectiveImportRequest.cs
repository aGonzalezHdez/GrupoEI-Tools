namespace Application.DTOs.Manifiestos.Import;

public sealed record CorrectiveImportRequest(
    ImportFileType FileType,
    string FileName,
    string ContentBase64,
    List<string>? ColumnsToUpdate,
    bool OverwriteEmpty = false,
    bool DryRun = true,
    bool HasHeader = true,   // CSV
    string Delimiter = ","   // CSV
);