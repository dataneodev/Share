using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vero.Shared.Logging;
using Vero.Shared.Security;

namespace Vero.Shared.MediatR
{
    public sealed class LoggingPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
        private readonly IContextAccessor _accessor;
        private readonly ILogger<LoggingPipelineBehavior<TRequest, TResponse>> _logger;
        private readonly LoggingOptions _options;

        public LoggingPipelineBehavior(IContextAccessor accessor, ILogger<LoggingPipelineBehavior<TRequest, TResponse>> logger, LoggingOptions options)
        {
            _logger = logger;
            _options = options;
            _accessor = accessor;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var props = request!.GetType()
                .GetProperties()
                .ToDictionary(k => k.Name, v => v.GetValue(request, null));

            var userContext = $"[UserId={(!_accessor.IsSystem ? _accessor.Details.UserId : "SYSTEM")}]";

            var requestLog = $"{userContext} Started handling of {typeof(TRequest).Name}";
            if (_options.Request && props.Any())
                requestLog += $" with properties: {JsonConvert.SerializeObject(props, Formatting.Indented)}";

            _logger.LogInformation(requestLog);

            try
            {
                var response = await next();

                var responseLog = $"{userContext} Successfully handled {typeof(TRequest).Name}";

                _logger.LogInformation(responseLog);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{userContext} Error when handling {typeof(TRequest).Name}");
                throw;
            }
        }
    }
}