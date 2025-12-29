namespace Daunt.Shared;

public sealed record ValidationResult(bool IsValid, IEnumerable<ValidationPropertyResult> Errors);