namespace Vero.Shared.DashboardStats
{
    public interface ITotalDayStatsOfT<T>
    {
        public DateOnly Day { get; set; }

        public T Total { get; set; }
    }
}