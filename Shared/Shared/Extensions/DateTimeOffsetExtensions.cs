namespace Vero.Shared.Extensions
{
    public static class DateTimeOffsetExtensions
    {
        public static string ToFormatedDateOnlyString(this DateTimeOffset date)
        {
            return date.ToString("dd.MM.yyyy");
        }

        public static int GetDaysBetween(this DateTimeOffset current, DateTimeOffset right)
        {
            return Math.Max((current.Date - right.Date).Days, 0);
        }

        public static bool IsBetweenTime(this DateTimeOffset current, TimeOnly Start, TimeOnly End)
        {
            var currentTime = TimeOnly.FromDateTime(current.DateTime);

            return currentTime >= Start && currentTime <= End;
        }

        /// <summary>
        ///     Sets the time of the current date with minute precision.
        /// </summary>
        /// <param name="current">The current date.</param>
        /// <param name="hour">The hour.</param>
        /// <returns>A DateTimeOffset.</returns>
        public static DateTimeOffset SetTime(this DateTimeOffset current, int hour)
        {
            return SetTime(current, hour, 0, 0, 0);
        }

        /// <summary>
        ///     Sets the time of the current date with minute precision.
        /// </summary>
        /// <param name="current">The current date.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <returns>A DateTimeOffset.</returns>
        public static DateTimeOffset SetTime(this DateTimeOffset current, int hour, int minute)
        {
            return SetTime(current, hour, minute, 0, 0);
        }

        /// <summary>
        ///     Sets the time of the current date with second precision.
        /// </summary>
        /// <param name="current">The current date.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <returns>A DateTimeOffset.</returns>
        public static DateTimeOffset SetTime(this DateTimeOffset current, int hour, int minute, int second)
        {
            return SetTime(current, hour, minute, second, 0);
        }

        /// <summary>
        ///     Sets the time of the current date with millisecond precision.
        /// </summary>
        /// <param name="current">The current date.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="millisecond">The millisecond.</param>
        /// <returns>A DateTimeOffset.</returns>
        public static DateTimeOffset SetTime(
            this DateTimeOffset current,
            int hour,
            int minute,
            int second,
            int millisecond
        )
        {
            return current.LocalDateTime.Date.AddHours(hour)
                .AddMinutes(minute)
                .AddSeconds(second)
                .AddMilliseconds(millisecond)
                .ToUniversalTime();
        }

        public static DateOnly GetDateOnly(this DateTimeOffset current)
        {
            return DateOnly.FromDateTime(current.DateTime);
        }

        public static DateTimeOffset GetStartOfDay(this DateTimeOffset current)
        {
            return current.UtcDateTime.Date;
        }

        public static DateTimeOffset GetEndOfDay(this DateTimeOffset current)
        {
            return current.UtcDateTime.Date.AddHours(23)
                .AddMinutes(59)
                .AddSeconds(59);
        }

        public static DateTimeOffset ConvertDateTimeOffsetToCentralEurope(this DateTimeOffset current)
        {
            var centralEuropeTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            return TimeZoneInfo.ConvertTime(current, centralEuropeTimeZone);
        }

        public static DateTimeOffset? ConvertDateTimeOffsetToCentralEurope(this DateTimeOffset? current)
        {
            if (!current.HasValue)
                return null;

            var centralEuropeTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            return TimeZoneInfo.ConvertTime(current.Value, centralEuropeTimeZone);
        }

        public static DateTimeOffset ReplaceTimeZoneToWarsaw(this DateTimeOffset current)
        {
            var zoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            var offset = zoneInfo.GetUtcOffset(current.LocalDateTime);
            return new DateTimeOffset(DateTime.SpecifyKind(current.LocalDateTime, DateTimeKind.Unspecified), offset).ToUniversalTime();
        }

        [Obsolete("Nie używać!")]
        public static DateTimeOffset? ReplaceTimeZoneToWarsaw(this DateTimeOffset? current)
        {
            return current.HasValue ? current.Value.ReplaceTimeZoneToWarsaw() : null;
        }

        [Obsolete("Nie używać")]
        public static DateTimeOffset? ReplaceTimeZoneWarsawToUTC(this DateTimeOffset? current)
        {
            return current.HasValue ? ReplaceTimeZoneWarsawToUTC(current.Value) : null;
        }

        [Obsolete("Nie używać")]
        public static DateTimeOffset ReplaceTimeZoneWarsawToUTC(this DateTimeOffset current)
        {
            var infotime = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            var offset = infotime.GetUtcOffset(current.LocalDateTime);

            if (offset ==
                current.ToLocalTime()
                    .Offset)
                return current;

            return new DateTimeOffset(DateTime.SpecifyKind(current.UtcDateTime, DateTimeKind.Unspecified), -offset).ToUniversalTime();
        }
    }
}