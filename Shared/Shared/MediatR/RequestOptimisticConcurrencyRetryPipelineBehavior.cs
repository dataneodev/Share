using System.Security.Cryptography;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vero.Shared.EntityFramework;

namespace Vero.Shared.MediatR
{
    public sealed class RequestOptimisticConcurrencyRetryPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
        private const int RetryCount = 6;
        private readonly IDBContextCleaner _dBContextCleaner;

        public RequestOptimisticConcurrencyRetryPipelineBehavior(IDBContextCleaner dBContextCleaner)
        {
            _dBContextCleaner = dBContextCleaner;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) =>
            await NextWithOptimisticConcurrencyRetryAsync(next, cancellationToken);

        private async Task<TResponse> NextWithOptimisticConcurrencyRetryAsync(
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken,
            int retry = 0
        )
        {
            try
            {
                return await next();
            }
            catch (DbUpdateConcurrencyException)
            {
                var nextRetry = retry + 1;
                if (nextRetry > RetryCount)
                    throw;

                if (cancellationToken.IsCancellationRequested)
                    throw;

                var delay = CalculateDelayForOptimisticConcurrencyRetry(nextRetry);
                await Task.Delay(delay, cancellationToken);

                _dBContextCleaner.CleanDBContext(cancellationToken);

                return await NextWithOptimisticConcurrencyRetryAsync(next, cancellationToken, nextRetry);
            }
        }

        private int CalculateDelayForOptimisticConcurrencyRetry(int retry)
        {
            if (retry <= 0)
                retry = 1;

            if (retry > RetryCount)
                retry = RetryCount;

            return RandomNumberGenerator.GetInt32(100 * (retry - 1), retry * 450);
        }
    }
}