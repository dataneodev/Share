using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ProfiBiznes.Shared.Infrastructure.Filters
{
    public sealed class OperationCancelledExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is not OperationCanceledException)
                return;

            context.ExceptionHandled = true;
            context.Result = new StatusCodeResult(StatusCodes.Status499ClientClosedRequest);
        }
    }
}