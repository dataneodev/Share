namespace Vero.Shared.Exceptions
{
    public sealed class ForeignLanguageEmptyException : AppException
    {
        public ForeignLanguageEmptyException() : base("Tłumaczenie na język obcy nie może być puste!")
        {
        }
    }
}