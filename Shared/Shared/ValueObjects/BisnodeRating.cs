using Vero.Shared.DDD;
using Vero.Shared.Exceptions;

namespace Vero.Shared.ValueObjects
{
    public sealed record BisnodeRating : ValueObjectOf<int>
    {
        public BisnodeRating(int Value) : base(Value)
        {
            if (Value < 1 || Value > 5)
                throw new InvalidValueOfBisnodeRatingException(Value);
        }
    }

    public sealed class InvalidValueOfBisnodeRatingException : AppException
    {
        public InvalidValueOfBisnodeRatingException(int value) : base($"Wartość {value} jest niepoprawną wartością rankingu bisnode!")
        {
        }
    }
}