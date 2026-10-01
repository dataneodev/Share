using System.Reflection;
using MassTransit;

namespace Vero.Shared.MessageBroker
{
    public static class ConfigureConsumers
    {
        public static IEnumerable<Consumer> GetConsumers(this IEnumerable<Assembly> assemblies)
        {
            var types = assemblies.SelectMany(GetAllConsumersFromAssembly);
            var consumers = types.Select(CreateConsumer);
            return consumers;
        }

        public static void AddConsumers(this IBusRegistrationConfigurator config, IEnumerable<Consumer> consumers)
        {
            foreach (var consumer in consumers)
            {
                config.AddConsumer(consumer.Type, consumer.Definition)
                    .Endpoint(c => c.Name = consumer.Name);
            }
        }

        private static IEnumerable<Type> GetAllConsumersFromAssembly(this Assembly assembly)
        {
            var types = assembly.GetTypes();
            return types.Where(
                t => t.IsClass &&
                     t.GetInterfaces()
                         .Any(i => i.Name == typeof(IConsumer<>).Name)
            );
        }

        private static Consumer CreateConsumer(Type type)
        {
            var consumerType = ConsumerType.Other;

            var baseType = type.BaseType;
            if (baseType != null &&
                baseType.IsConstructedGenericType &&
                baseType.GenericTypeArguments.Any() &&
                baseType.Name == typeof(RequestConsumer<,>).Name)
                consumerType = ConsumerType.Request;

            return consumerType switch
            {
                ConsumerType.Request => new Consumer(type, typeof(RequestBusConsumerDefinition<>).MakeGenericType(type)),
                ConsumerType.Other => new Consumer(type, typeof(DefaultConsumerDefinition<>).MakeGenericType(type)),
                _ => throw new InvalidOperationException("Zaimplementuj dodatkowe wartości ConsumerType w GetConsumerWithDefinition")
            };
        }

        private enum ConsumerType : byte
        {
            Request,
            Other
        }

        public readonly struct Consumer
        {
            public readonly Type Type;
            public readonly Type Definition;
            public readonly string Name;

            public Consumer(Type consumer, Type definition)
            {
                Type = consumer;
                Definition = definition;
                Name = consumer.FullName!;
            }
        }
    }
}