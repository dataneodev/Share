namespace Vero.Shared.DashboardStats
{
    public sealed record HalfYearTotalCalculatorOfT<T>(T Total, T Increase)
        where T : struct;

    public sealed class HalfYearTotalCalculatorOfT<T, T2>
        where T : ITotalDayStatsOfT<T2>
        where T2 : struct
    {
        private readonly Func<T2, T2, T2> _delta;

        public HalfYearTotalCalculatorOfT(Func<T2, T2, T2> delta)
        {
            _delta = delta;
        }

        public HalfYearTotalCalculatorOfT<T2> CalculateStats(List<T> days)
        {
            var sorted = days.OrderBy(o => o.Day)
                .ToList();


            var halfYearPast = DateOnly.FromDateTime(
                DateTimeOffset.UtcNow.AddMonths(-6)
                    .DateTime
            );

            var ordersYearPast = days.MinBy(m => m.Day);
            var ordersHalfYars = days.Where(w => w.Day >= halfYearPast)
                .MinBy(m => m.Day);

            var currentOrder = days.MaxBy(m => m.Day);

            var previousSixMonth = default(T2);
            var currentSixMonth = default(T2);

            if (ordersYearPast is not null && ordersHalfYars is not null && ordersYearPast.Day < halfYearPast)
            {
                previousSixMonth = _delta(ordersHalfYars.Total, ordersYearPast.Total);
            }

            if (currentOrder is not null && ordersHalfYars is not null)
            {
                currentSixMonth = _delta(ordersHalfYars.Total, currentOrder.Total);
            }


            return new HalfYearTotalCalculatorOfT<T2>(currentSixMonth, _delta(previousSixMonth, currentSixMonth));
        }
    }
}