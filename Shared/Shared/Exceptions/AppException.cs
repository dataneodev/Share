namespace Vero.Shared.Exceptions
{
    [Serializable]
    public abstract class AppException(string message) : Exception(message);
}