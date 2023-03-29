namespace Persistify.Validators.Common;

public record ValidationResult(bool IsValid, string? Message = null);