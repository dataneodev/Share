using System.Security.Cryptography;
using MassTransit;
using Microsoft.Extensions.Logging;
using Vero.Shared.Abstractions.DDD;
using Vero.Shared.MartenExtensions;
using ConcurrencyException = Marten.Exceptions.ConcurrencyException;

namespace Vero.Shared.Events
{
    internal interface IProjector<in T> : IConsumer<T>
        where T : IntegrationEvent
    {
        Task Project(T projection);
    }

    public abstract class Projector<T> : IProjector<T>
        where T : IntegrationEvent
    {
        private const int RetryCount = 6;

        protected readonly ILogger<Projector<T>> _logger;

        protected Projector(ILogger<Projector<T>> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<T> context)
        {
            var name = context.Message.GetType()
                           ?.FullName ??
                       typeof(T).FullName;

            try
            {
                _logger.LogInformation("Projecting message {Name}", name);

                await ProjectWithOptimisticConcurrencyRetryAsync(context.Message);

                _logger.LogInformation("Successfully projected message {Name}", name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error projecting message {Name}", name);
                throw;
            }
        }

        public abstract Task Project(T projection);

        private async Task ProjectWithOptimisticConcurrencyRetryAsync(T projection, int retry = 0)
        {
            try
            {
                await Project(projection);
            }
            catch (Exception ex) when (ex is ItemToPatchNotFoundException || ex is ConcurrencyException)
            {
                var nextRetry = retry + 1;
                if (nextRetry > RetryCount)
                    throw;

                var delay = CalculateDelayForOptimisticConcurrencyRetry(nextRetry);

                await Task.Delay(delay);
                await ProjectWithOptimisticConcurrencyRetryAsync(projection, nextRetry);
            }
        }

        private int CalculateDelayForOptimisticConcurrencyRetry(int retry)
        {
            if (retry <= 0)
                retry = 1;

            if (retry > RetryCount)
                retry = RetryCount;

            return RandomNumberGenerator.GetInt32(retry * 100, retry * 400);
        }
    }
}