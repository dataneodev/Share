namespace Vero.Shared.Extensions
{
    public static class DateOnlyExtensions
    {
        public static string ToFormatedString(this DateOnly date)
        {
            return date.ToString("dd.MM.yyyy");
        }

        public static DateTimeOffset ToDateTimeOffset(this DateOnly dateOnly)
        {
            return dateOnly.ToDateTime(new TimeOnly(12, 00), DateTimeKind.Utc);
        }

        // return new DateTimeOffset(dateTime, zone.GetUtcOffset(dateTime));
        public static DateTimeOffset? ToDateTimeOffset(this DateOnly? dateOnly)
        {
            return dateOnly?.ToDateTime(new TimeOnly(12, 00), DateTimeKind.Utc);
        }
        // return new DateTimeOffset(dateTime, zone.GetUtcOffset(dateTime));
    }
}