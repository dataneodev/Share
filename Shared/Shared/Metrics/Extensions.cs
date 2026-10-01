using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;

namespace Vero.Shared.Metrics
{
    public static class Extensions
    {
        internal static void AddVeroMetrics(this IServiceCollection services)
        {
            services.AddMetrics();
            services.AddHealthChecks()
                .ForwardToPrometheus();
        }

        internal static void UseVeroMetrics(this IApplicationBuilder app)
        {
            app.UseMetricServer();
            app.UseHttpMetrics();
            app.UseHealthChecks("/health", new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });
        }
    }
}