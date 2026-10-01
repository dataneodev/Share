using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Grafana.Loki;

namespace Vero.Shared.Logging
{
    public static class LoggingExtensions
    {
        public static IHostBuilder UseAppLogging(this IHostBuilder hostBuilder) => hostBuilder.UseSerilog(
            (ctx, lc) =>
            {
                lc.ConfigureMinLevel(ctx);
                lc.Enrich.FromLogContext();
                ConfigureSinks(lc, ctx);
                lc.ReadFrom.Configuration(ctx.Configuration);
            }
        );
        
        private static void ConfigureMinLevel(this LoggerConfiguration lc, HostBuilderContext ctx)
        {
            if (ctx.HostingEnvironment.IsDevelopment())
            {
                lc.MinimumLevel.Debug();
                lc.MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
                lc.MinimumLevel.Override("Quartz", LogEventLevel.Information);
                lc.MinimumLevel.Override("MassTransit", LogEventLevel.Information);
                if (Debugger.IsAttached)
                {
                    lc.MinimumLevel.Verbose();
                    lc.MinimumLevel.Override("Microsoft", LogEventLevel.Debug);
                    lc.MinimumLevel.Override("Quartz", LogEventLevel.Debug);
                    lc.MinimumLevel.Override("MassTransit", LogEventLevel.Debug);
                }
                return;
            }
            lc.MinimumLevel.Information();
            lc.MinimumLevel.Override("Microsoft", LogEventLevel.Information);
            lc.MinimumLevel.Override("Quartz", LogEventLevel.Information);
            lc.MinimumLevel.Override("MassTransit", LogEventLevel.Information);
        }
        private static void ConfigureSinks(LoggerConfiguration lc, HostBuilderContext ctx)
        {
            var name = Assembly.GetEntryAssembly()
                           ?.GetName()
                           .Name
                           ?.Split('.')[0] ??
                       "unknown";
            lc.WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
            lc.Enrich.WithProperty("Application", ctx.HostingEnvironment.ApplicationName);
            lc.Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName);
            lc.WriteTo.GrafanaLoki(
                ctx.HostingEnvironment.IsDevelopment() ? "http://localhost:3100" : "http://vero-loki:3100",
                credentials: null,
                period: TimeSpan.FromSeconds(10),
                propertiesAsLabels: new List<string>
                {
                    "level",
                    "CorrelationId",
                    "InitiatorId",
                    "Message",
                    "RequestPath",
                    "RequestName",
                    "IntegrationEventHandler",
                    "CommandHandler",
                    "QueryHandler",
                    "Exception",
                    "ErrorStatusCode",
                    "ErrorCode",
                    "LiveNotificationType",
                    "ConnectionId"
                },
                labels: new List<LokiLabel> { new() { Key = "Microservice", Value = name } }
            );
        }
    }
}