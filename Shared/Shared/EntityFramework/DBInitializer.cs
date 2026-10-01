using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;
using Vero.Shared.EntityFramework.Converters;

namespace Vero.Shared.EntityFramework
{
    public static class DBInitializerExtensions
    {
        public static IServiceCollection AddDBInitializer(this IServiceCollection services) => services;

        public static IServiceCollection AddEntityFramework<T>(this IServiceCollection services, DatabaseOptions options)
            where T : DbContext
        {
            services.AddSingleton(options);
            services.AddDbContext<T>(
                o =>
                {
                    o.UseNpgsql(
                        options.ConnectionString,
                        builder =>
                        {
                            builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                            builder.EnableRetryOnFailure(10, TimeSpan.FromSeconds(5), null);
                            builder.CommandTimeout(30);
                        }
                    );

                    o.ReplaceService<IValueConverterSelector, EntityIdValueConverterSelector>();
                    o.EnableDetailedErrors();
                    o.EnableSensitiveDataLogging();
                }
            );

            services.AddDBInitializer();
            return services;
        }
    }
}