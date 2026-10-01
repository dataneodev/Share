using Vero.Shared.Extensions;

namespace Vero.Shared.Exceptions
{
    public sealed class ApiException
    {
        public ApiException(Exception ex, Dictionary<string, string[]>? errors = null)
        {
            ErrorCode = ex.GetType()
                .Name.Replace("Exception", string.Empty)
                .ToSnakeCase();

            Message = ex.Message;
            Errors = errors ?? new();
        }

        public ApiException(string message, string errorCode, Dictionary<string, string[]>? errors = null)
        {
            ErrorCode = errorCode;
            Message = message;
            Errors = errors ?? new();
        }

        public string ErrorCode { get; }

        public string Message { get; }

        public Dictionary<string, string[]>? Errors { get; }
    }
}