using Vero.Shared.DDD;
using Vero.Shared.Exceptions;

namespace Vero.Shared.ValueObjects
{
    public sealed record TimePeriod(TimePeriodHours Hours, TimePeriodMinutes Minutes);

    public sealed record TimePeriodHours : ValueObjectOf<int>
    {
        public TimePeriodHours(int value) : base(value)
        {
            if (value < 0)
                throw new InvalidTimePeriodHoursException("Godziny nie mogą być ujemne");
        }
    }

    public sealed class InvalidTimePeriodHoursException : AppException
    {
        public InvalidTimePeriodHoursException(string message) : base(message)
        {
        }
    }

    public sealed record TimePeriodMinutes : ValueObjectOf<int>
    {
        public TimePeriodMinutes(int value) : base(value)
        {
            switch (value)
            {
                case < 0:
                    throw new InvalidTimePeriodMinutesException("Minuty nie mogą być ujemne!");

                case > 59:
                    throw new InvalidTimePeriodMinutesException("Minuty nie mogą być większe niż 59!");
            }
        }
    }

    public sealed class InvalidTimePeriodMinutesException : AppException
    {
        public InvalidTimePeriodMinutesException(string message) : base(message)
        {
        }
    }

    public static class TimePeriodExtensions
    {
        public static int ToMinutes(this TimePeriod timePeriod) => timePeriod.Hours.Value * 60 + timePeriod.Minutes.Value;

        public static TimePeriod ToTimePeriod(this int minutes) => new(new TimePeriodHours(minutes / 60), new TimePeriodMinutes(minutes % 60));
    }
}