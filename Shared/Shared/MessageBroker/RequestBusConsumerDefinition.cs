using MassTransit;

namespace Vero.Shared.MessageBroker
{
    public sealed class RequestBusConsumerDefinition<TInput> : ConsumerDefinition<TInput>
        where TInput : class, IConsumer
    {
        protected override void ConfigureConsumer(
            IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<TInput> consumerConfigurator,
            IRegistrationContext context
        )
        {
            consumerConfigurator.ConcurrentMessageLimit = 1;
            consumerConfigurator.UseTimeout(t => t.Timeout = TimeSpan.FromSeconds(15));
        }
    }
}