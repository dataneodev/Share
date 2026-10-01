using System.Reflection;
using Marten;
using MassTransit;
using MassTransit.MartenIntegration.Saga;
using Vero.Shared.EntityFramework;
using Vero.Shared.Extensions;
using Weasel.Core;

namespace Vero.Shared.StateMachines
{
    public  static  class RegisterStateMachines
    {
        private readonly struct StateMachines
        {
            public readonly Type StateMachine;
            public readonly Type StateMachineInstance;

            public StateMachines(Type stateMachine, Type stateMachineInstance)
            {
                StateMachine = stateMachine;
                StateMachineInstance = stateMachineInstance;
            }
        }

        public const string SchemaName = "sagas";

        public static void AddStateMachines(
            this IRabbitMqReceiveEndpointConfigurator rabbitMqReceiveEndpointConfigurator,
            DatabaseOptions databaseOptions,
            params Assembly[] assemblies
        )
        {
            foreach (var machine in GetStateMachinesTypes(assemblies))
            {
                var stateMachineIstance = GetIstanceOfStateMachine(machine.StateMachine);
                var repository = GetMartenSagaRepository(machine.StateMachineInstance, databaseOptions);

                var mi = typeof(RegisterStateMachines).GetMethod("AddMachineSaga");
                var stateMachineSagaRef = mi!.MakeGenericMethod(machine.StateMachineInstance);

                stateMachineSagaRef.Invoke(null, new object[] { rabbitMqReceiveEndpointConfigurator, stateMachineIstance, repository });
            }
        }

        private static IEnumerable<StateMachines> GetStateMachinesTypes(IEnumerable<Assembly> assemblies) => assemblies.SelectMany(GetStateMachinesTypes);

        private static IEnumerable<StateMachines> GetStateMachinesTypes(Assembly assemblie)
        {
            var baseMachineType = typeof(MassTransitStateMachine<>);
            return baseMachineType.GetGenericWithGenericArgumentType(assemblie)
                .Select(t => new StateMachines(t.Key, t.Value));
        }

        private static dynamic GetIstanceOfStateMachine(Type type) => Activator.CreateInstance(type)!;

        private static dynamic GetMartenSagaRepository(Type type, DatabaseOptions databaseOptions)
        {
            var options = new StoreOptions { AutoCreateSchemaObjects = AutoCreate.All, DatabaseSchemaName = SchemaName };

            options.Connection(databaseOptions.ConnectionString);
            options.CreateDatabasesForTenants(
                opt => opt.ForTenant()
                    .WithOwner("postgres")
                    .WithEncoding("UTF-8")
                    .ConnectionLimit(-1)
            );

            var docStore = new DocumentStore(options);
            var memoryRespository = typeof(MartenSagaRepository<>);
            var memoryRespositoryTyped = memoryRespository.MakeGenericType(type);
            var create = memoryRespositoryTyped.GetMethod("Create");

            return create!.Invoke(null, new object[] { docStore })!;
        }
    }
}