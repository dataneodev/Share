using System.Globalization;
using System.Reflection;
using JasperFx.CodeGeneration;
using Marten;
using Marten.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Vero.Shared.Extensions;
using Weasel.Core;

namespace Vero.Shared.MartenExtensions
{
    public static class MartenIServiceCollectionExtensions
    {
        public static IServiceCollection AddMartenConfiguration<T>(this IServiceCollection services, string connectionString)
            where T : class, IConfigureMarten
        {
            services.AddMarten(
                o =>
                {
                    o.Connection(connectionString);

                    o.GeneratedCodeMode = TypeLoadMode.Auto;
                    o.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
                    o.DatabaseSchemaName = "projections";
                    o.Policies.DisableInformationalFields();

                    var serializer = new JsonNetSerializer();
                    serializer.Configure(
                        _ =>
                        {
                            _.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                            _.Converters.Add(new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeLocal });
                        }
                    );

                    o.Serializer(serializer);
                }
            );

            services.AddSingleton<IConfigureMarten, T>();

            return services;
        }

        public static IServiceCollection AddMartenConfigurations(this IServiceCollection services, string connectionString)
        {
            return services.AddMartenConfigurations(Assembly.GetCallingAssembly(), connectionString);
        }

        public static IServiceCollection AddMartenConfigurations(this IServiceCollection services, Assembly assembly, string connectionString)
        {
            services.AddMarten(
                o =>
                {
                    o.Connection(connectionString);
                    o.GeneratedCodeMode = TypeLoadMode.Auto;

                    o.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
                    o.DatabaseSchemaName = "projections";
                    o.Policies.DisableInformationalFields();

                    var serializer = new JsonNetSerializer();
                    serializer.Configure(
                        _ =>
                        {
                            _.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                            _.Converters.Add(new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeLocal });
                        }
                    );

                    o.Serializer(serializer);
                }
            );

            var types = assembly.GetTypesImplementingInterface<IConfigureMarten>();

            foreach (var type in types)
                services.AddSingleton(typeof(IConfigureMarten), type);

            return services;
        }
    }
}