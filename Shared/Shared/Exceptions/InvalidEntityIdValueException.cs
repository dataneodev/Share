namespace Vero.Shared.Exceptions
{
    public sealed class InvalidEntityIdValueException : AppException
    {
        public InvalidEntityIdValueException() : base("Niepoprawna wartość indentyfikatora obiektu")
        {
        }
    }
}