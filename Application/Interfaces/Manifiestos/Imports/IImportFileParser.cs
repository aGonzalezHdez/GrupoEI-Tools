using Application.DTOs.Manifiestos.Import;

namespace Application.Interfaces.Manifiestos.Imports;

public interface IImportFileParser
{
    ImportFileType FileType { get; }
    Task<List<ImportedRow>> ParseAsync(Stream file, CorrectiveImportRequest request, CancellationToken ct);
}