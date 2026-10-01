using Vero.Shared.DashboardStats;
using Xunit;

namespace Shared.UnitTests.DashboardStats
{
    public sealed class TotalDayStatsCalculatorTestsOfT
    {
        [Fact]
        public void Test()
        {
            var calculator = new TotalDayStatsCalculatorOfT<TotalDayStats, double>((last, first) => Math.Max(last - first, 0));

            var totalDays = new List<TotalDayStats>
            {
                new(new DateOnly(2024, 01, 01), 15),
                new(new DateOnly(2024, 01, 02), 15),
                new(new DateOnly(2024, 01, 05), 20),
                new(new DateOnly(2024, 01, 15), 50),
                new(new DateOnly(2024, 01, 25), 88),
                new(new DateOnly(2024, 02, 15), 150)
            };

            var stats = calculator.CalculateStats(totalDays);
        }

        [Fact]
        public void TestEmpty()
        {
            var calculator = new TotalDayStatsCalculatorOfT<TotalDayStats, double>((last, first) => Math.Max(last - first, 0));

            var totalDays = new List<TotalDayStats>();

            var stats = calculator.CalculateStats(totalDays);
        }

        [Fact]
        public void TestOldValue()
        {
            var calculator = new TotalDayStatsCalculatorOfT<TotalDayStats, double>((last, first) => Math.Max(last - first, 0));

            var totalDays = new List<TotalDayStats> { new(new DateOnly(2022, 01, 01), 1) };

            var stats = calculator.CalculateStats(totalDays);
        }

        private sealed class TotalDayStats : ITotalDayStatsOfT<double>
        {
            public TotalDayStats(DateOnly day, double total)
            {
                Day = day;
                Total = total;
            }

            public DateOnly Day { get; set; }

            public double Total { get; set; }
        }
    }
}