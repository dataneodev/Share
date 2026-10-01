namespace Vero.Shared.DashboardStats
{
    internal sealed class TotalDayStatsOfTComparer<T, T2> : IComparer<T>
        where T2 : struct
        where T : ITotalDayStatsOfT<T2>, new()
    {
        private readonly IComparer<DateOnly?> _defaultComparer = Comparer<DateOnly?>.Default;

        public int Compare(T? x, T? y) => _defaultComparer.Compare(x?.Day, y?.Day);
    }
}
