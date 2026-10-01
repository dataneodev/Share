using Vero.Shared.DDD;
using Vero.Shared.Exceptions;

namespace Vero.Shared.ValueObjects
{
    public sealed record Time : ValueObject
    {
        public Time(byte hour, byte minute)
        {
            if (Hour > 23)
                throw new InvalidHourException();

            if (Hour > 59)
                throw new InvalidMinuteException();

            Hour = hour;
            Minute = minute;
        }

        public byte Hour { get; }

        public byte Minute { get; }
    }

    public sealed class InvalidHourException : AppException
    {
        public InvalidHourException() : base("Niepoprawna godzina")
        {
        }
    }

    public sealed class InvalidMinuteException : AppException
    {
        public InvalidMinuteException() : base("Niepoprawne minuty czasu")
        {
        }
    }
}