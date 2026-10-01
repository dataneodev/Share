using Vero.Shared.Exceptions;

namespace Vero.Shared.Validation
{
    public sealed class ValidatorException : AppException
    {
        public ValidatorException(Dictionary<string, string[]> errors) : base("Błąd walidacji")
        {
            Errors = errors;
        }

        public Dictionary<string, string[]> Errors { get; }
    }
}