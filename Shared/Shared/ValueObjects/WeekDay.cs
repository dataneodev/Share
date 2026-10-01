using Newtonsoft.Json;
using Vero.Shared.DDD;

namespace Vero.Shared.ValueObjects
{
    public sealed class WeekDay : Enumeration<WeekDay>
    {
        public static readonly WeekDay Monday = new(1, "Poniedziałek");
        public static readonly WeekDay Tuesday = new(2, "Wtorek");
        public static readonly WeekDay Wednesday = new(3, "Środa");
        public static readonly WeekDay Thursday = new(4, "Czwartek");
        public static readonly WeekDay Friday = new(5, "Piątek");
        public static readonly WeekDay Saturday = new(6, "Sobota");
        public static readonly WeekDay Sunday = new(7, "Niedziela");

        [JsonConstructor]
        private WeekDay(int id, string value) : base(id, value)
        {
        }

        public static WeekDay From(System.DayOfWeek dayOfWeek) => WeekDay.FromId(dayOfWeek == 0 ? 7 : (int)dayOfWeek);
    }
}