using MassTransit;
using Microsoft.Extensions.Logging;

namespace Vero.Shared.MessageBroker
{
    public sealed class ConsumeObserver : IConsumeObserver
    {
        private readonly ILogger<ConsumeObserver> _logger;

        public ConsumeObserver(ILogger<ConsumeObserver> logger)
        {
            _logger = logger;
        }

        public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception)
            where T : class => Task.CompletedTask;

        public Task PostConsume<T>(ConsumeContext<T> context)
            where T : class
        {
            _logger.LogInformation($"Consumed context {context.Message?.GetType()}");
            return Task.CompletedTask;
        }

        public Task PreConsume<T>(ConsumeContext<T> context)
            where T : class
        {
            _logger.LogInformation($"Started consuming context {context.Message.GetType()}");
            return Task.CompletedTask;
        }
    }
}