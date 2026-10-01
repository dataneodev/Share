using System.Globalization;
using System.Reflection;
using AutoMapper;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;
using Vero.Shared.Abstractions;
using Vero.Shared.Abstractions.Bus;
using Vero.Shared.Abstractions.DDD;
using Vero.Shared.AutoMapper;
using Vero.Shared.EntityFramework;
using Vero.Shared.Events;
using Vero.Shared.Extensions;
using Vero.Shared.Json;
using Vero.Shared.MediatR;
using Vero.Shared.MessageBroker;
using Vero.Shared.Metrics;
using Vero.Shared.Middleware;
using Vero.Shared.ModelBinders;
using Vero.Shared.Options;
using Vero.Shared.Security;
using Vero.Shared.StateMachines;
using Vero.Shared.Swagger;
using Vero.Shared.Validation;

namespace Vero.Shared
{
    public static class Module
    {
        public static IServiceCollection AddLogging(this IServiceCollection services)
        {
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));
            return services;
        }

        public static IServiceCollection AddRequestValidation(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipeline<,>));

            foreach (var assembly in assemblies)
                services.AddValidatorsFromAssembly(assembly);

            return services;
        }

        public static IServiceCollection AddDomainEvents<TUoW>(this IServiceCollection services, Dictionary<Type, Type> events)
            where TUoW : DbContext
        {
            services.AddScoped<IDomainEventsDispatcher, DomainEventsDispatcher>();
            services.AddScoped<IDomainEventsMapper>(service => new DomainEventsMapper(service.GetRequiredService<IMapper>(), events));
            services.AddScoped(typeof(IDomainEventsProvider), typeof(DomainEventsProvider<TUoW>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(IntegrationEventsWithResponseDispatcherPipelineBehavior<,>));

            return services;
        }

        public static IServiceCollection AddUnitOfWork<T>(this IServiceCollection services)
            where T : DbContext
        {
            services.AddScoped(typeof(IDBContextCleaner), typeof(DBContextCleaner<T>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestOptimisticConcurrencyRetryPipelineBehavior<,>));

            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork<T>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkPipeline<,>));

            return services;
        }

        public static IServiceCollection AddContextAccessor(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IContextAccessor, ContextAccessor>();

            return services;
        }

        public static IServiceCollection AddBus(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));
            services.AddSingleton<ICommandBus, CommandBus>();
            services.AddSingleton<IQueryBus, QueryBus>();
            services.AddSingleton<IEventBus, EventBus>();

            return services;
        }

        public static IServiceCollection AddExceptionMiddleware(this IServiceCollection services)
        {
            return services.AddScoped<ExceptionMiddleware>();
        }

        public static IServiceCollection AddShared(
            this IServiceCollection services,
            SharedOptions options,
            AssembliesContainer assemblies,
            IConfiguration configuration
        )
        {
            if (options.UseSwagger)
            {
                services.AddSwaggerDocumentation(options.SwaggerTypesForPolymorphism, assemblies);
            }

            services.AddControllers(o => o.ModelBinderProviders.Insert(0, new DateTimeOffsetModelBinderProvider()))
                .AddNewtonsoftJson(
                    c =>
                    {
                        c.SerializerSettings.ContractResolver = JsonConfig.ContractResolver();
                        c.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                        c.SerializerSettings.Converters.Add(
                            new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeLocal }
                        );
                    }
                );

            services.AddExceptionMiddleware();
            services.AddContextAccessor();

            services.AddAuth(options.Auth);

            if (options.UseBus)
            {
                if (assemblies.Application is not null)
                    services.AddBus(assemblies.Application);

                if (assemblies.Infrastructure is not null)
                    services.AddBus(assemblies.Infrastructure);
            }

            if (options.Logging.Enabled)
            {
                services.AddLogging();
                services.AddSingleton(options.Logging);
            }

            if (options.UseValidation)
            {
                if (assemblies.Application is not null)
                    services.AddRequestValidation(assemblies.Application);

                if (assemblies.Infrastructure is not null)
                    services.AddRequestValidation(assemblies.Infrastructure);
            }

            services.AddDateOnlyTimeOnlyStringConverters();
            services.AddVeroHealthChecks(configuration);

            return services;
        }

        public static IServiceCollection AddUploadMaxFileSizeConfiguration(this IServiceCollection services)
        {
            const int MaxUploadFileSize = 536870912;//512MB

            services.Configure<FormOptions>(
                options =>
                {
                    options.ValueLengthLimit = MaxUploadFileSize;
                    options.MultipartBodyLengthLimit = MaxUploadFileSize;
                    options.MultipartBoundaryLengthLimit = MaxUploadFileSize;
                }
            );

            services.Configure<IISServerOptions>(options => { options.MaxRequestBodySize = MaxUploadFileSize; });

            services.Configure<KestrelServerOptions>(options => { options.Limits.MaxRequestBodySize = MaxUploadFileSize; });

            return services;
        }

        public static IServiceCollection AddShared<TUoW>(
            this IServiceCollection services,
            IConfiguration configuration,
            SharedOptions options,
            EventProfile eventProfile,
            AssembliesContainer assembliesContainer
        )
            where TUoW : DbContext
        {
            eventProfile.AddValueObjectOfMap(assembliesContainer.Domain!, assembliesContainer.Shared!);
            eventProfile.AddEnumerationMap(assembliesContainer.Domain!, assembliesContainer.Shared!);
            eventProfile.AddDomainEventToIntegrationEventMap(
                assembliesContainer.Domain!,
                assembliesContainer.Contract!,
                options.ThrowOnEventMappingNotFound,
                options.MapNestedEventTypes
            );

            services.AddHttpClient();
            services.AddMapper(eventProfile);
            services.AddMessageBroker(configuration, assembliesContainer);
            services.AddShared(options, assembliesContainer, configuration);
            services.AddSingleton(options);
            services.AddVeroMetrics();

            if (options.Events.Any())
            {
                services.AddDomainEvents<TUoW>(options.Events);
            }

            if (options.UseUnitOfWork)
            {
                services.AddUnitOfWork<TUoW>();
            }

            services.AddVeroHealthChecks(configuration);

            return services;
        }

        private static IServiceCollection AddVeroHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks();

            return services;
        }

        public static IServiceCollection AddMessageBroker(
            this IServiceCollection services,
            IConfiguration configuration,
            AssembliesContainer assembliesContainer
        )
        {
            var databaseOptions = configuration.GetOptions<DatabaseOptions>("Database");
            var options = configuration.GetOptions<MessageBrokerOptions>("MessageBroker");
            var assemblies = new[] { assembliesContainer.Application!, assembliesContainer.Infrastructure! };

            //services.AddConsumeObserver<ConsumeObserver>();
            services.AddMassTransit(
                config =>
                {
                    var consumers = assemblies.GetConsumers();
                    config.AddConsumers(consumers);
                    config.SetKebabCaseEndpointNameFormatter();
                    config.UsingRabbitMq(
                        (context, cfg) =>
                        {
                            cfg.AutoStart = true;

                            cfg.Host(
                                options.HostName,
                                options.Port,
                                "/",
                                h =>
                                {
                                    h.Username(options.UserName);
                                    h.Password(options.Password);
                                }
                            );

                            foreach (var consumer in consumers)
                            {
                                cfg.ReceiveEndpoint(
                                    consumer.Name,
                                    c =>
                                    {
                                        c.AddStateMachines(databaseOptions, assemblies);
                                        c.ConfigureConsumer(context, consumer.Type);
                                    }
                                );
                            }

                            cfg.Publish<IRequestBus>(c => c.Exclude = true);
                            cfg.Publish<INotification>(c => c.Exclude = true);
                            cfg.Publish<IIntegrationEvent>(c => c.Exclude = true);

                            cfg.ConfigureJsonSerializerOptions(
                                options =>
                                {
                                    options.Converters.Add(new DateOnlyConverter());
                                    options.Converters.Add(new TimeOnlyConverter());
                                    return options;
                                }
                            );
                        }
                    );
                }
            );

            return services;
        }

        public static IApplicationBuilder UseShared(this IApplicationBuilder app, bool swagger = true)
        {
            Task.Delay(Constants.DelayAppStartup);

            app.UseSerilogRequestLogging();
            if (swagger)
                app.UseSwaggerDocumentation();

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapHealthChecks("/healthz");
                }
            );

            app.UseVeroMetrics();

            return app;
        }
    }
}