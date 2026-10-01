using MassTransit;

namespace Vero.Shared.MessageBroker
{
    public sealed class DefaultConsumerDefinition<TInput> : ConsumerDefinition<TInput>
        where TInput : class, IConsumer
    {
        protected override void ConfigureConsumer(
            IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<TInput> consumerConfigurator,
            IRegistrationContext context
        )
        {
            consumerConfigurator.ConcurrentMessageLimit = 1;

            consumerConfigurator.UseMessageRetry(
                r => r.Intervals(
                    TimeSpan.FromMilliseconds(50),
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(20),
                    TimeSpan.FromSeconds(30),
                    TimeSpan.FromMinutes(1),
                    TimeSpan.FromMinutes(5),
                    TimeSpan.FromMinutes(10)
                )
            );

            consumerConfigurator.UseMessageRetry(
                r => r.Intervals(
                    TimeSpan.FromMilliseconds(50),
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(20),
                    TimeSpan.FromSeconds(30),
                    TimeSpan.FromMinutes(1),
                    TimeSpan.FromMinutes(5),
                    TimeSpan.FromMinutes(10)
                )
            );

            consumerConfigurator.UseDelayedRedelivery(
                r => r.Intervals(
                    TimeSpan.FromMinutes(30),
                    TimeSpan.FromHours(1),
                    TimeSpan.FromHours(2),
                    TimeSpan.FromHours(6),
                    TimeSpan.FromHours(12),
                    TimeSpan.FromDays(1),
                    TimeSpan.FromDays(2),
                    TimeSpan.FromDays(5),
                    TimeSpan.FromDays(7)
                )
            );

            consumerConfigurator.UseInMemoryOutbox(context);
        }
    }
}