

namespace .Shared.Infrastructure
{
    public static class Module
    {
        public static void AddShared(this IServiceCollection services, AddSharedOptions options) =>
            services.AddShared(options, options.Assemblies, options.ConfigureMassTransit, options.ConfigureRabbitMq);

        public static void AddShared<TDbContext>(this IServiceCollection services, AddSharedOptions options)
            where TDbContext : DbContext
        {
            services.AddShared(options);

            if (options.Database is not null)
            {
                services.AddEntityFramework<TDbContext>(options.Database);
                services.AddScoped<IRepository, Repository>();
                services.AddScoped<IIdGeneratorFactory, IdGeneratorFactory>();
                services.AddScoped(_ => new DomainRepositoryProvider(options.Assemblies));

                if (options.Database.IsMultiTenant)
                {
                    services.AddScoped<ICompanyTenantRepository>(_ => new CompanyTenantRepository(options.Database.ConnectionString));
                    services.AddScoped<ICompaniesRepository>(_ => new CompaniesRepository(options.Database.ConnectionString));
                    services.AddScoped<ICompanyDatabaseCreator, CompanyDatabaseCreator>();
                }

                if (options.UseScheduler)
                    services.AddScheduler(options.Database.ConnectionString);
            }

            if (options.Mapping is not null)
                services.AddDomainEvents(options.Mapping);
        }

        public static void UseShared(this IApplicationBuilder app, UseSharedOptions? options = null)
        {
            app.UseSerilogRequestLogging();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseHsts();
            app.UseRouting();
            app.UseProfiBiznesMetrics();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseTranslations();

            if (options?.UseScheduler == true)
            {
                app.UseScheduler();
            }

            app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapMetrics();

                    if (options?.UseScheduler == true)
                        endpoints.MapHangfireDashboard();

                    options?.ConfigureEndpoints?.Invoke(endpoints);
                }
            );
        }

        private static void AddCQRS(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            services.AddMediatR(x => x.RegisterServicesFromAssemblies(assemblies.ToArray()));
            services.AddSingleton<ICommandBus, CommandBus>();
            services.AddSingleton<IQueryBus, QueryBus>();
        }

        private static void AddDomainEvents(this IServiceCollection services, Func<DomainEvent, Initiator, Guid, CompanyId?, IntegrationEvent?> mapping)
        {
            services.AddScoped<IDomainEventsDispatcher, DomainEventsDispatcher>();
            services.AddScoped<IDomainEventsProvider, DomainEventsProvider>();
            services.AddScoped<IDomainToIntegrationEventMapper>(_ => new DomainToIntegrationEventMapper(mapping));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(DomainEventsPipeline<,>));
        }

        private static void AddExceptionMiddleware(this IServiceCollection services) => services.AddScoped<ExceptionMiddleware>();

        private static void AddShared(
            this IServiceCollection services,
            AddSharedOptions options,
            AssembliesContainer assemblies,
            Action<IBusRegistrationConfigurator>? configureMassTransit = null,
            Action<IRabbitMqBusFactoryConfigurator>? configureRabbitMq = null
        )
        {
            services.AddProfiBiznesMetrics();
            services.AddTranslations();
            services.AddExceptionMiddleware();
            services.AddAuth(options);
            services.AddCQRS(assemblies.Available);

            services.AddControllers(x =>
                    {
                        x.Filters.Add(new ProducesResponseTypeAttribute(typeof(ApiException), StatusCodes.Status400BadRequest));
                        x.Filters.Add(new ProducesResponseTypeAttribute(typeof(ApiException), StatusCodes.Status401Unauthorized));
                        x.Filters.Add(new ProducesResponseTypeAttribute(typeof(ApiException), StatusCodes.Status403Forbidden));
                        x.Filters.Add<OperationCancelledExceptionFilter>();
                    }
                )
                .AddNewtonsoftJson(c =>
                    {
                        c.SerializerSettings.ContractResolver = JsonConfig.SerializerSettings.ContractResolver;
                        c.SerializerSettings.Formatting = JsonConfig.SerializerSettings.Formatting;
                        c.SerializerSettings.ConstructorHandling = JsonConfig.SerializerSettings.ConstructorHandling;
                        c.SerializerSettings.DateTimeZoneHandling = JsonConfig.SerializerSettings.DateTimeZoneHandling;
                        c.SerializerSettings.DateParseHandling = JsonConfig.SerializerSettings.DateParseHandling;
                    }
                );


            if (options.UseBus)
            {
                services.AddScoped<IIntegrationEventPublisher, IntegrationEventPublisher>();
                services.AddScoped<IIntegrationEventScheduler, IntegrationEventScheduler>();
                services.AddMessageBroker(options.Configuration, options.Assemblies, configureMassTransit, configureRabbitMq);
            }

            services.AddFusionCacheManager();
        }
    }
}