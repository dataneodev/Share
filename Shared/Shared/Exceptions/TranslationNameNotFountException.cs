namespace Vero.Shared.Exceptions
{
    public sealed class TranslationNameNotFountException : AppException
    {
        public TranslationNameNotFountException(string? name = "") : base($"Nie znaleziono nazwy tłumaczenia {name} dla wybranego języka")
        {
        }
    }
}