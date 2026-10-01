using MassTransit;
using Vero.Shared.Abstractions.Bus;

namespace Vero.Shared.Events
{
    public abstract class Dispatcher<T> : IConsumer<T>
        where T : class, IIntegrationEvent
    {
        private readonly IEventBus _events;

        public Dispatcher(IEventBus events)
        {
            _events = events;
        }

        public virtual Task Consume(ConsumeContext<T> context) => _events.Publish(context.Message);
    }
}