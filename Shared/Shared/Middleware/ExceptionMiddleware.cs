using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Vero.Shared.Exceptions;
using Vero.Shared.Validation;

namespace Vero.Shared.Middleware
{
    public sealed class ExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next) => await RunExceptionPipeline(context, next);

        public async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            switch (ex)
            {
                case UnauthorizedAccessException unauthorizedAccessException:
                {
                    await HandleExceptionAsync(context, StatusCodes.Status401Unauthorized, new ApiException(unauthorizedAccessException), ex);

                    break;
                }

                case ValidatorException validatorException:
                {
                    await HandleExceptionAsync(
                        context,
                        StatusCodes.Status400BadRequest,
                        new ApiException(GetValidatorExceptionMessage(validatorException), "400", validatorException.Errors),
                        ex
                    );

                    break;
                }

                case NoPermissionException noPermissionException:
                {
                    await HandleExceptionAsync(context, StatusCodes.Status403Forbidden, new ApiException(noPermissionException), ex);

                    break;
                }

                case BusinessRuleValidationException businessRuleValidationException:
                {
                    await HandleExceptionAsync(
                        context,
                        StatusCodes.Status400BadRequest,
                        new ApiException(businessRuleValidationException.Details, businessRuleValidationException.Code),
                        ex
                    );

                    break;
                }

                case AppException appException:
                {
                    await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, new ApiException(appException), ex);

                    break;
                }

                default:
                {
                    await HandleExceptionAsync(
                        context,
                        StatusCodes.Status500InternalServerError,
                        new ApiException("Wystąpił niespodziewany błąd serwera!", "internal_server_error"),
                        ex
                    );

                    break;
                }
            }
        }

        private async Task RunExceptionPipeline(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, int status, ApiException apiException, Exception baseException)
        {
            _logger.LogError(baseException, apiException.ErrorCode, apiException.Message, apiException.Errors);

            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(apiException);
        }

        private static string GetValidatorExceptionMessage(ValidatorException exception)
        {
            var msg = string.Empty;

            if (!string.IsNullOrWhiteSpace(exception.Message))
                msg = $"{exception.Message}: ";

            msg += string.Join(" ,", exception.Errors.SelectMany(s => s.Value));

            return msg;
        }
    }
}