using Prometheus;
using Serilog;
using Vero.Shared.Extensions;
using ITimer = Prometheus.ITimer;

namespace Vero.Shared.Metrics
{
    //plan
    //Zrobić interfejsy na metryki (przekazywać je do app a tą klasę przenieść do infrastruktury)
    //Zrobić bazową klasę metryki, która będzie miała metody typu CreateMetricName, labele i np. tworzenie initiatorów itp
    public static class VeroMetrics
    {
        private static string[] CreateCommonLabels(CommonLabels labels) =>
        [
            labels.CorrelationId?.ToString() ?? string.Empty,
            labels.InitiatorId?.ToString() ?? string.Empty,
        ];

        private static string CreateMetricName<T>() =>
            typeof(T).FullName
                ?.Replace("+", string.Empty)
                .Replace($"{typeof(T).Namespace}.", "")
                .ToSnakeCase() ??
            string.Empty;

        private sealed record CommonLabels
        {
            public Guid? CorrelationId { get; init; }
            
            public int? InitiatorId { get; init; }
            
        }

        private static class Labels
        {
            public static readonly string CorrelationId = nameof(CorrelationId);
            public static readonly string InitiatorId = nameof(InitiatorId);
            public static readonly string[] Common = [CorrelationId, InitiatorId];
            public static readonly string Exception = nameof(Exception);

            public static readonly string IntegrationEventHandler = nameof(IntegrationEventHandler);
            public static readonly string CommandHandler = nameof(CommandHandler);
            public static readonly string QueryHandler = nameof(QueryHandler);
            public static readonly string ConnectionId = nameof(ConnectionId);
            public static readonly string LiveNotificationType = nameof(LiveNotificationType);
        }

        public abstract class Hubs
        {
            private static readonly Gauge _gauge = Prometheus.Metrics.CreateGauge(
                CreateMetricName<Hubs>(),
                "Current number of active hub connections",
                new GaugeConfiguration { LabelNames = [..Labels.Common, Labels.ConnectionId] }
            );

            public sealed class Connected
            {
                public static void Record(int initiatorId, string connectionId)
                {
                    Log.Information("[{InitiatorId}]][{ConnectionId}] {Message}", initiatorId,  connectionId, "Connected to hub");

                    _gauge.WithLabels([..CreateCommonLabels(new CommonLabels { InitiatorId = initiatorId}), connectionId])
                        .Inc();
                }
            }

            public sealed class LiveNotificationsSent
            {
                private static readonly Counter _counter = Prometheus.Metrics.CreateCounter(
                    CreateMetricName<LiveNotificationsSent>(),
                    "Total number of live notifications sent",
                    new CounterConfiguration { LabelNames = [..Labels.Common, Labels.LiveNotificationType] }
                );

                public static void Record(Guid correlationId, string liveNotificationType, List<string> connections)
                {
                    Log.Information(
                        "[{CorrelationId}][{LiveNotificationType}]: {Message}",
                        correlationId,
                        liveNotificationType,
                        $"Sent message to connections {string.Join(", ", connections)}"
                    );

                    _counter.WithLabels([..CreateCommonLabels(new CommonLabels { CorrelationId = correlationId }), liveNotificationType])
                        .Inc();
                }
            }

            public sealed class Disconnected
            {
                public static void Record(int initiatorId, string connectionId)
                {
                    Log.Information(
                        "[{InitiatorId}][{ConnectionId}] {Message}",
                        initiatorId,
                        connectionId,
                        "Disconnected from hub"
                    );

                    _gauge.WithLabels([..CreateCommonLabels(new CommonLabels { InitiatorId = initiatorId }), connectionId])
                        .Dec();
                }
            }
        }

        public abstract class IntegrationEventHandlers
        {
            private static readonly string[] _labels = [..Labels.Common, Labels.IntegrationEventHandler];
            private static readonly Gauge _gauge = Prometheus.Metrics.CreateGauge(
                CreateMetricName<IntegrationEventHandlers>(),
                "Total number of running integration event handlers",
                new GaugeConfiguration { LabelNames = [.._labels] }
            );

            public sealed class IntegrationEventHandlersSuccess
            {
                private static readonly Counter _counter = Prometheus.Metrics.CreateCounter(
                    CreateMetricName<IntegrationEventHandlersSuccess>(),
                    "Total number of completed integration event handlers",
                    new CounterConfiguration { LabelNames = _labels }
                );

                public static void Record(string integrationEventHandlerName, Guid correlationId, int? initiatorId)
                {
                    Log.Information(
                        "[{IntegrationEventHandler}][{CorrelationId}][{InitiatorId}] {Message}",
                        integrationEventHandlerName,
                        correlationId,
                        initiatorId,
                        "Successfully handled event"
                    );

                    _counter.WithLabels(
                            [
                                ..CreateCommonLabels(new CommonLabels { CorrelationId = correlationId, InitiatorId = initiatorId }),
                                integrationEventHandlerName
                            ]
                        )
                        .Inc();
                }
            }

            public sealed class IntegrationEventHandlersRun
            {
                private static readonly Counter _counter = Prometheus.Metrics.CreateCounter(
                    CreateMetricName<IntegrationEventHandlersRun>(),
                    "Total number of ran integration event handlers",
                    new CounterConfiguration { LabelNames = _labels }
                );

                public static void Record(string integrationEventHandlerName, Guid correlationId, int? initiatorId)
                {
                    Log.Information(
                        "[{IntegrationEventHandler}][{CorrelationId}][{InitiatorId}] {Message}",
                        integrationEventHandlerName,
                        correlationId,
                        initiatorId,
                        "Finished handling event"
                    );

                    _counter.WithLabels(
                            [
                                ..CreateCommonLabels(new CommonLabels { CorrelationId = correlationId, InitiatorId = initiatorId }),
                                integrationEventHandlerName
                            ]
                        )
                        .Inc();

                    _gauge.WithLabels(
                            [
                                ..CreateCommonLabels(new CommonLabels { CorrelationId = correlationId, InitiatorId = initiatorId }),
                                integrationEventHandlerName
                            ]
                        )
                        .Dec();
                }
            }

            public sealed class IntegrationEventHandlersFail
            {
                private static readonly Counter _counter = Prometheus.Metrics.CreateCounter(
                    CreateMetricName<IntegrationEventHandlersFail>(),
                    "Total number of failed integration event handlers",
                    new CounterConfiguration { LabelNames = [.._labels, Labels.Exception] }
                );

                public static void Record(
                    Exception ex,
                    string integrationEventHandlerName,
                    Guid correlationId,
                    int? initiatorId
                )
                {
                    Log.Error(
                        ex,
                        "[{IntegrationEventHandler}][{CorrelationId}][{InitiatorId}][{Exception}] {Message}",
                        integrationEventHandlerName,
                        correlationId,
                        initiatorId,
                        ex.Message,
                        "Failed handling event"
                    );

                    _counter.WithLabels(
                            [
                                ..CreateCommonLabels(new CommonLabels { CorrelationId = correlationId, InitiatorId = initiatorId }),
                                integrationEventHandlerName,
                                ex.Message
                            ]
                        )
                        .Inc();
                }
            }

            public sealed class IntegrationEventHandlersDuration
            {
                private static readonly Histogram _timer = Prometheus.Metrics.CreateHistogram(
                    CreateMetricName<IntegrationEventHandlersDuration>(),
                    "Duration of integration event handlers",
                    new HistogramConfiguration { LabelNames = _labels }
                );

                public static ITimer Record(string integrationEventHandlerName, Guid correlationId, int? initiatorId)
                {
                    Log.Information(
                        "[{IntegrationEventHandler}][{CorrelationId}][{InitiatorId}] {Message}",
                        integrationEventHandlerName,
                        correlationId,
                        initiatorId,
                        "Starting event handling.."
                    );

                    _gauge.WithLabels(
                            [
                                ..CreateCommonLabels(new CommonLabels { CorrelationId = correlationId, InitiatorId = initiatorId }),
                                integrationEventHandlerName
                            ]
                        )
                        .Inc();


                    return _timer.WithLabels(
                            [
                                ..CreateCommonLabels(new CommonLabels { CorrelationId = correlationId, InitiatorId = initiatorId }),
                                integrationEventHandlerName
                            ]
                        )
                        .NewTimer();
                }
            }
        }

        public abstract class CommandHandlers
        {
            private static readonly string[] _labels = [..Labels.Common, Labels.CommandHandler];

            private static readonly Gauge _gauge = Prometheus.Metrics.CreateGauge(
                CreateMetricName<CommandHandlers>(),
                "Total number of running command handlers",
                new GaugeConfiguration { LabelNames = [.._labels] }
            );

            public sealed class CommandHandlersSuccess
            {
                private static readonly Counter _counter = Prometheus.Metrics.CreateCounter(
                    CreateMetricName<CommandHandlersSuccess>(),
                    "Total number of completed command handlers",
                    new CounterConfiguration { LabelNames = _labels }
                );

                public static void Record(string commandHandlerName, Guid correlationId, int? initiatorId)
                {
                    Log.Information(
                        "[{CommandHandler}][{CorrelationId}][{InitiatorId}] {Message}",
                        commandHandlerName,
                        correlationId,
                        initiatorId,
                        "Successfully handled command"
                    );

                    _counter.WithLabels(
                            [
                                ..CreateCommonLabels(new CommonLabels { CorrelationId = correlationId, InitiatorId = initiatorId }),
                                commandHandlerName
                            ]
                        )
                        .Inc();
                }
            }

            public sealed class CommandHandlersRun
            {
                private static readonly Counter _counter = Prometheus.Metrics.CreateCounter(
                    CreateMetricName<CommandHandlersRun>(),
                    "Total number of ran command handlers",
                    new CounterConfiguration { LabelNames = _labels }
                );

                public static void Record(string commandHandlerName, Guid correlationId, int? initiatorId)
                {
                    Log.Information(
                        "[{CommandHandler}][{CorrelationId}][{InitiatorId}] {Message}",
                        commandHandlerName,
                        correlationId,
                        initiatorId,
                        "Finished handling command"
                    );


                    _counter.WithLabels(
                            [
                                ..CreateCommonLabels(new CommonLabels { CorrelationId = correlationId, InitiatorId = initiatorId}),
                                commandHandlerName
                            ]
                        )
                        .Inc();

                    _gauge.WithLabels(
                            [
                                ..CreateCommonLabels(new CommonLabels { CorrelationId = correlationId, InitiatorId = initiatorId }),
                                commandHandlerName
                            ]
                        )
                        .Dec();
                }
            }

            public sealed class CommandHandlersFail
            {
                private static readonly Counter _counter = Prometheus.Metrics.CreateCounter(
                    CreateMetricName<CommandHandlersFail>(),
                    "Total number of failed command handlers",
                    new CounterConfiguration { LabelNames = [.._labels, Labels.Exception] }
                );

                public static void Record(
                    Exception ex,
                    string commandHandlerName,
                    Guid correlationId,
                    int? initiatorId
                )
                {
                    Log.Error(
                        ex,
                        "[{CommandHandler}][{CorrelationId}][{InitiatorId}][{Exception}] {Message}",
                        commandHandlerName,
                        correlationId,
                        initiatorId,
                        ex.Message,
                        "Failed handling command"
                    );

                    _counter.WithLabels(
                            [
                                ..CreateCommonLabels(new CommonLabels { CorrelationId = correlationId, InitiatorId = initiatorId }),
                                commandHandlerName,
                                ex.Message
                            ]
                        )
                        .Inc();
                }
            }

            public sealed class CommandHandlersDuration
            {
                private static readonly Histogram _timer = Prometheus.Metrics.CreateHistogram(
                    CreateMetricName<CommandHandlersDuration>(),
                    "Duration of command handlers",
                    new HistogramConfiguration { LabelNames = _labels }
                );

                public static ITimer Record(string commandHandlerName, Guid correlationId, int? initiatorId)
                {
                    Log.Information(
                        "[{CommandHandler}][{CorrelationId}][{InitiatorId}] {Message}",
                        commandHandlerName,
                        correlationId,
                        initiatorId,
                        "Starting command handling.."
                    );

                    _gauge.WithLabels(
                            [
                                ..CreateCommonLabels(new CommonLabels { CorrelationId = correlationId, InitiatorId = initiatorId}),
                                commandHandlerName
                            ]
                        )
                        .Inc();

                    return _timer.WithLabels(
                            [
                                ..CreateCommonLabels(new CommonLabels { CorrelationId = correlationId, InitiatorId = initiatorId}),
                                commandHandlerName
                            ]
                        )
                        .NewTimer();
                }
            }
        }

        public abstract class QueryHandlers
        {
            private static readonly string[] _labels = [..Labels.Common, Labels.QueryHandler];

            private static readonly Gauge _gauge = Prometheus.Metrics.CreateGauge(
                CreateMetricName<QueryHandlers>(),
                "Total number of running query handlers",
                new GaugeConfiguration { LabelNames = [.._labels] }
            );

            public sealed class QueryHandlersSuccess
            {
                private static readonly Counter _counter = Prometheus.Metrics.CreateCounter(
                    CreateMetricName<QueryHandlersSuccess>(),
                    "Total number of completed query handlers",
                    new CounterConfiguration { LabelNames = _labels }
                );

                public static void Record(string queryHandlerName, int? initiatorId)
                {
                    Log.Information(
                        "[{QueryHandler}][{InitiatorId}] {Message}",
                        queryHandlerName,
                        initiatorId,
                        "Successfully handled query"
                    );

                    _counter.WithLabels([..CreateCommonLabels(new CommonLabels { InitiatorId = initiatorId }), queryHandlerName])
                        .Inc();
                }
            }

            public sealed class QueryHandlersRun
            {
                private static readonly Counter _counter = Prometheus.Metrics.CreateCounter(
                    CreateMetricName<QueryHandlersRun>(),
                    "Total number of ran query handlers",
                    new CounterConfiguration { LabelNames = _labels }
                );

                public static void Record(string queryHandlerName, int? initiatorId)
                {
                    Log.Information(
                        "[{QueryHandler}][{InitiatorId}] {Message}",
                        queryHandlerName,
                        initiatorId,
                        "Finished handling query"
                    );

                    _counter.WithLabels([..CreateCommonLabels(new CommonLabels { InitiatorId = initiatorId }), queryHandlerName])
                        .Inc();

                    _gauge.WithLabels([..CreateCommonLabels(new CommonLabels { InitiatorId = initiatorId}), queryHandlerName])
                        .Dec();
                }
            }

            public sealed class QueryHandlersFail
            {
                private static readonly Counter _counter = Prometheus.Metrics.CreateCounter(
                    CreateMetricName<QueryHandlersFail>(),
                    "Total number of failed query handlers",
                    new CounterConfiguration { LabelNames = [.._labels, Labels.Exception] }
                );

                public static void Record(Exception ex, string queryHandlerName, int? initiatorId)
                {
                    Log.Error(
                        ex,
                        "[{QueryHandler}][{InitiatorId}][{Exception}] {Message}",
                        queryHandlerName,
                        initiatorId,
                        ex.Message,
                        "Failed handling query"
                    );

                    _counter.WithLabels([..CreateCommonLabels(new CommonLabels { InitiatorId = initiatorId}), queryHandlerName, ex.Message])
                        .Inc();
                }
            }

            public sealed class QueryHandlersDuration
            {
                private static readonly Histogram _timer = Prometheus.Metrics.CreateHistogram(
                    CreateMetricName<QueryHandlersDuration>(),
                    "Duration of query handlers",
                    new HistogramConfiguration { LabelNames = _labels }
                );

                public static ITimer Record(string queryHandlerName, int? initiatorId)
                {
                    Log.Information(
                        "[{QueryHandler}][{InitiatorId}]] {Message}",
                        queryHandlerName,
                        initiatorId,
                        "Starting query handling.."
                    );

                    _gauge.WithLabels([..CreateCommonLabels(new CommonLabels { InitiatorId = initiatorId}), queryHandlerName])
                        .Inc();

                    return _timer.WithLabels([..CreateCommonLabels(new CommonLabels { InitiatorId = initiatorId}), queryHandlerName])
                        .NewTimer();
                }
            }
        }
    }
}