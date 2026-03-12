namespace Application.DTOs.Manifiestos.Import;

public enum ImportLineStatus
{
    Updated,
    Skipped,
    Warning,
    Error
}

public sealed record ImportLineResult(
    int LineNumber,
    string? GuiaHouse,
    ImportLineStatus Status,
    List<string> Messages
);

public sealed record ImportSummary(
    int TotalLines,
    int Updated,
    int Skipped,
    int Warnings,
    int Errors
);

public sealed record ImportReport(
    DateTime StartedUtc,
    DateTime FinishedUtc,
    ImportSummary Summary,
    List<ImportLineResult> Lines
);