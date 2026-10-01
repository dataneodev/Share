using MassTransit;
using Microsoft.Extensions.Logging;

namespace Vero.Shared.MessageBroker
{
    public abstract class RequestConsumer<TRequest, TResponse> : IConsumer<TRequest>
        where TRequest : class, IRequestBus<TResponse>
        where TResponse : class
    {
        private readonly ILogger<RequestConsumer<TRequest, TResponse>> _logger;

        public RequestConsumer(ILogger<RequestConsumer<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TRequest> context)
        {
            _logger.LogInformation(
                "Started handling request {Name}",
                context.Message.GetType()
                    ?.Name
            );

            var responseValue = await ConsumeRequestAsync(context);
            await context.RespondAsync(responseValue);
            _logger.LogInformation(
                "Successfully response to request {Name}",
                context.Message.GetType()
                    ?.Name
            );
        }

        public abstract Task<TResponse> ConsumeRequestAsync(ConsumeContext<TRequest> context);
    }
}