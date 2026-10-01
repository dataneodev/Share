using MassTransit;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vero.Shared.Abstractions.Bus;
using Vero.Shared.Abstractions.DDD;
using Vero.Shared.Metrics;

namespace Vero.Shared.MessageBroker
{
    public abstract class IntegrationEventHandler<TEvent> : IConsumer<TEvent>
        where TEvent : IntegrationEvent
    {
        private readonly ILogger<IntegrationEventHandler<TEvent>> _logger;

        public IntegrationEventHandler(ILogger<IntegrationEventHandler<TEvent>> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TEvent> context)
        {
            var name = GetType()
                .FullName!;
            
            var correlationId = context.Message.CorrelationId;
            var initiator = context.Message.InitiatorId;

            try
            {
                await Handle(context.Message);
                VeroMetrics.IntegrationEventHandlers.IntegrationEventHandlersSuccess.Record(name, correlationId, initiator);
            }
            catch (Exception ex)
            {
                VeroMetrics.IntegrationEventHandlers.IntegrationEventHandlersFail.Record(ex, name, correlationId, initiator);
                throw;
            }
            finally
            {
                VeroMetrics.IntegrationEventHandlers.IntegrationEventHandlersRun.Record(name, correlationId, initiator);
            }
        }

        protected abstract Task Handle(TEvent @event);
    }

    internal sealed class EventBus : IEventBus
    {
        private readonly IServiceScopeFactory _scope;

        public EventBus(IServiceScopeFactory scope)
        {
            _scope = scope;
        }

        public async Task Publish(IIntegrationEvent @event)
        {
            using var scope = _scope.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Publish(@event);
        }
    }
}