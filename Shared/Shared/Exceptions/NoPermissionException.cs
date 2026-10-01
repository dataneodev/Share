namespace Vero.Shared.Exceptions
{
    public sealed class NoPermissionException : AppException
    {
        public NoPermissionException() : base("Brak uprawnień do wykonania akcji!")
        {
        }

        public NoPermissionException(string message) : base(message)
        {
        }
    }
}