namespace Vero.Shared.Helpers
{
    public static class RetryAction
    {
        public static async Task ExecuteWithRetryAsync(Func<Task> action, int retryCount = 5, int delay = 1500, CancellationToken cancellationToken = default)
        {
            for (var i = 0; i < retryCount; i++)
            {
                try
                {
                    await action();
                    return;
                }
                catch (Exception)
                {
                    if (i == retryCount - 1 || cancellationToken.IsCancellationRequested)
                    {
                        throw;
                    }
                }

                cancellationToken.ThrowIfCancellationRequested();

                if (delay > 0)
                {
                    await Task.Delay(delay)
                        .ConfigureAwait(false);
                }
            }
        }

        public static async Task<T> GetWithRetryAsync<T>(
            Func<Task<T>> task,
            int retryCount = 5,
            int delay = 1500,
            CancellationToken cancellationToken = default
        )
        {
            for (var i = 0; i < retryCount; i++)
            {
                try
                {
                    return await task();
                }
                catch (Exception)
                {
                    if (i == retryCount - 1 || cancellationToken.IsCancellationRequested)
                    {
                        throw;
                    }
                }

                cancellationToken.ThrowIfCancellationRequested();

                if (delay > 0)
                {
                    await Task.Delay(delay)
                        .ConfigureAwait(false);
                }
            }

            throw new InvalidOperationException();
        }
    }
}