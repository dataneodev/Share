using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Vero.Shared
{
    public abstract class TimedHostedService(ILogger<TimedHostedService> logger) : IHostedService, IDisposable
    {
        protected readonly ILogger _logger = logger;
        protected readonly CancellationTokenSource _stoppingCts = new();
        private volatile Task? _executingTask;
        private volatile Timer _timer;

        protected abstract TimeSpan Period { get; }

        public void Dispose()
        {
            _stoppingCts.Cancel();
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            _timer = new Timer(ExecuteTask, null, Period, TimeSpan.FromMilliseconds(-1));

            return Task.CompletedTask;
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);

            // Stop called without start
            if (_executingTask == null)
            {
                return;
            }

            try
            {
                // Signal cancellation to the executing method
                _stoppingCts.Cancel();
            }
            finally
            {
                // Wait until the task completes or the stop token triggers
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
                _executingTask = null;
            }
        }

        /// <summary>
        ///     This method is called when the <see cref="IHostedService" /> starts. The implementation should return a task
        /// </summary>
        /// <param name="stoppingToken">Triggered when <see cref="IHostedService.StopAsync(CancellationToken)" /> is called.</param>
        /// <returns>A <see cref="Task" /> that represents the long running operations.</returns>
        protected abstract Task RunJobAsync(CancellationToken stoppingToken);

        private void ExecuteTask(object? state)
        {
            _timer?.Change(Timeout.Infinite, 0);
            _executingTask = ExecuteTaskAsync(_stoppingCts.Token);
        }

        private async Task ExecuteTaskAsync(CancellationToken stoppingToken)
        {
            try
            {
                await RunJobAsync(stoppingToken);
            }
            finally
            {
                _timer.Change(Period, TimeSpan.FromMilliseconds(-1));
            }
        }
    }
}