using ProfiBiznes.Shared.Application;

namespace ProfiBiznes.Shared.UnitTests.Application
{
    public sealed class TimedBackgroundServiceImplMock : TimedBackgroundService
    {
        public int Counter { get; private set; }

        protected override TimeSpan Period => TimeSpan.FromMilliseconds(100);

        protected override async Task RunJobAsync(CancellationToken stoppingToken)
        {
            Counter++;
            await Task.Delay(TimeSpan.FromMilliseconds(600), stoppingToken);

            if (Counter > 2)
            {
                await StopAsync(stoppingToken);
            }
        }
    }
    // public sealed class TimedBackgroundServiceTests
    // {
    //     [Fact]
    //     public async Task TimedBackgroundService_RunJobAsync_RunsOnlyOnceInPeriod()
    //     {
    //         var service = new TimedBackgroundServiceImplMock();
    //         
    //         await service.StartAsync(new CancellationToken());
    //         
    //         await Task.Delay(TimeSpan.FromMilliseconds(1200));
    //         
    //         service.Counter
    //             .Should()
    //             .Be(2);
    //     }
    /// }
}