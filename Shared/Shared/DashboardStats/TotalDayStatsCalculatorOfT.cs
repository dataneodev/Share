using Vero.Shared.Extensions;

namespace Vero.Shared.DashboardStats
{
    public sealed record TotalDayStatsCalculatorResultOfT<T>(
        TotalDayStatsCalculatorResultOfT<T>.TotalDayStatsCalculatorResultPeriod<T> Week,
        TotalDayStatsCalculatorResultOfT<T>.TotalDayStatsCalculatorResultPeriod<T> Month,
        TotalDayStatsCalculatorResultOfT<T>.TotalDayStatsCalculatorResultPeriod<T> Year
    )
        where T : struct
    {
        public sealed record TotalDayStatsCalculatorResultPeriod<TP>(
            TP Total,
            IReadOnlyList<TotalDayStatsCalculatorResultPeriod<TP>.TotalDayStatsCalculatorResultPeriodItem<TP>> Data
        )
        {
            public sealed record TotalDayStatsCalculatorResultPeriodItem<TI>(DateTimeOffset Day, TI Value);
        }
    }

    public sealed class TotalDayStatsCalculatorOfT<T, T2>
        where T : ITotalDayStatsOfT<T2>
        where T2 : struct
    {
        private readonly Func<T2, T2, T2> _delta;

        public TotalDayStatsCalculatorOfT(Func<T2, T2, T2> delta)
        {
            _delta = delta;
        }

        public TotalDayStatsCalculatorResultOfT<T2> CalculateStats(List<T> days, DateTimeOffset from)
        {
            var sorted = days.OrderBy(o => o.Day)
                .ToList();

            return new TotalDayStatsCalculatorResultOfT<T2>(GetWeek(sorted, from), GetMonth(sorted, from), GetYear(sorted, from));
        }

        public TotalDayStatsCalculatorResultOfT<T2> CalculateStats(List<T> days) => CalculateStats(days, DateTimeOffset.UtcNow);

        private static IEnumerable<DateTimeOffset> GetPeriodsForWeek(DateTimeOffset from)
        {
            const int Periods = 6;

            var now = (DateTimeOffset)from.Date.AddHours(12)
                .AddDays(-Periods);

            return Enumerable.Range(0, Periods + 1)
                .Select(s => now.AddDays(s));
        }

        private static IEnumerable<DateTimeOffset> GetPeriodsForMonth(DateTimeOffset from)
        {
            var now = (DateTimeOffset)from.Date.AddHours(12)
                .AddDays(-31);

            yield return now;
            yield return now.AddDays(7);
            yield return now.AddDays(15);
            yield return now.AddDays(23);
            yield return now.AddDays(31);
        }

        private static IEnumerable<DateTimeOffset> GetPeriodsForYear(DateTimeOffset from)
        {
            var now = (DateTimeOffset)from.Date.AddHours(12)
                .AddYears(-1);

            yield return now;
            yield return now.AddDays(92);
            yield return now.AddDays(183);
            yield return now.AddDays(274);
            yield return now.AddYears(1);
        }

        private TotalDayStatsCalculatorResultOfT<T2>.TotalDayStatsCalculatorResultPeriod<T2> GetWeek(List<T> days, DateTimeOffset from)
        {
            var weekDays = GetPeriodsForWeek(from);
            var first = GetValueForDay(weekDays.First(), days, default);

            var periodItems = weekDays.SelectReadonlyList(
                s => new TotalDayStatsCalculatorResultOfT<T2>.TotalDayStatsCalculatorResultPeriod<T2>.TotalDayStatsCalculatorResultPeriodItem<T2>(
                    s,
                    GetValueForDay(s, days, first)
                )
            );

            var total = GetTotal(periodItems);
            return new TotalDayStatsCalculatorResultOfT<T2>.TotalDayStatsCalculatorResultPeriod<T2>(total, periodItems);
        }

        private TotalDayStatsCalculatorResultOfT<T2>.TotalDayStatsCalculatorResultPeriod<T2> GetMonth(List<T> days, DateTimeOffset from)
        {
            var weekDays = GetPeriodsForMonth(from);
            var first = GetValueForDay(weekDays.First(), days, default);

            var periodItems = weekDays.SelectReadonlyList(
                s => new TotalDayStatsCalculatorResultOfT<T2>.TotalDayStatsCalculatorResultPeriod<T2>.TotalDayStatsCalculatorResultPeriodItem<T2>(
                    s,
                    GetValueForDay(s, days, first)
                )
            );

            var total = GetTotal(periodItems);
            return new TotalDayStatsCalculatorResultOfT<T2>.TotalDayStatsCalculatorResultPeriod<T2>(total, periodItems);
        }

        private TotalDayStatsCalculatorResultOfT<T2>.TotalDayStatsCalculatorResultPeriod<T2> GetYear(List<T> days, DateTimeOffset from)
        {
            var weekDays = GetPeriodsForYear(from);
            var first = GetValueForDay(weekDays.First(), days, default);

            var periodItems = weekDays.SelectReadonlyList(
                s => new TotalDayStatsCalculatorResultOfT<T2>.TotalDayStatsCalculatorResultPeriod<T2>.TotalDayStatsCalculatorResultPeriodItem<T2>(
                    s,
                    GetValueForDay(s, days, first)
                )
            );

            var total = GetTotal(periodItems);
            return new TotalDayStatsCalculatorResultOfT<T2>.TotalDayStatsCalculatorResultPeriod<T2>(total, periodItems);
        }

        private T2 GetTotal(
            IReadOnlyList<TotalDayStatsCalculatorResultOfT<T2>.TotalDayStatsCalculatorResultPeriod<T2>.TotalDayStatsCalculatorResultPeriodItem<T2>> periodItems
        )
        {
            if (periodItems.Count == 1)
                return periodItems[0].Value;

            if (periodItems.Count == 0)
                return default;

            var total = _delta(periodItems[0].Value, periodItems[^1].Value);

            return total;
        }

        private T2 GetValueForDay(DateTimeOffset day, List<T> days, T2 firstValue)
        {
            var dayOnly = DateOnly.FromDateTime(day.Date);
            if (days.Count == 0)
                return default;

            var last = days.LastOrDefault(f => f.Day <= dayOnly);

            if (last == null)
                return default;

            return _delta(firstValue, last.Total);
        }
    }
}