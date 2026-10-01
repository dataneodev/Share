using Vero.Shared.DDD;

namespace Vero.Shared.ValueObjects
{
    public sealed record Days(int Value) : ValueObjectOf<int>(Value)
    {
        public static implicit operator TimeSpan(Days o) => TimeSpan.FromDays(o.Value);

        public static Days? Create(int? value) => value.HasValue ? new Days(value.Value) : null;

        public static string GetFormattedString(int days) => days == 1 ? "1 dzień" : $"{days} dni";
    }
}