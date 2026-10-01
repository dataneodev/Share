namespace Vero.Shared.DashboardStats
{
    public static class ITotalDayStatsOfTExtensions
    {
        public static void Increase<T, T2>(this List<T> stats, DateTimeOffset date, Func<DateOnly, T2, T2> update)
            where T2 : struct
            where T : ITotalDayStatsOfT<T2>, new()
        {
            AddToday(stats, date, update);

            CleanOldStats<T, T2>(stats, date);
        }

        private static void AddToday<T, T2>(List<T> stats, DateTimeOffset date, Func<DateOnly, T2, T2> update)
            where T2 : struct
            where T : ITotalDayStatsOfT<T2>, new()
        {
            var today = DateOnly.FromDateTime(date.DateTime);
            var day = stats.FirstOrDefault(f => f.Day == today);
            if (day is not null)
            {
                day.Total = update(day.Day, day.Total);
            }
            else
            {
                var last = stats.Where(w => w.Day < today)
                               .MaxBy(m => m.Day)
                               ?.Total ??
                           default;

                var newTotal = update(today, last);

                var newStats = new T();
                newStats.Day = today;
                newStats.Total = newTotal;

                stats.Add(newStats);
                stats.Sort(new TotalDayStatsOfTComparer<T, T2>());
            }

            foreach (var stat in stats.Where(w => w.Day > today))
            {
                if (stat is null)
                    continue;

                stat.Total = update(stat.Day, stat.Total);
            }
        }

        private static void CleanOldStats<T, T2>(List<T> stats, DateTimeOffset date)
            where T2 : struct
            where T : ITotalDayStatsOfT<T2>
        {
            var lastYear = DateOnly.FromDateTime(
                date.AddYears(-1)
                    .Date
            );

            stats.RemoveAll(r => r.Day < lastYear);
        }
    }
}
