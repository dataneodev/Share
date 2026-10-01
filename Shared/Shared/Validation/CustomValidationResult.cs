namespace Vero.Shared.Validation
{
    public sealed record CustomValidationResult(bool IsValid, string ErrorMessage, string FieldName);
}