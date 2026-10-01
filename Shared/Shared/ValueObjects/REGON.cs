using System.Text.RegularExpressions;
using Vero.Shared.DDD;
using Vero.Shared.Exceptions;

namespace Vero.Shared.ValueObjects
{
    public sealed record REGON : ValueObjectOf<string>
    {
        public REGON(string value) : base(value)
        {
            var valueTrimmed = value?.Trim();

            if (string.IsNullOrWhiteSpace(valueTrimmed) || !Regex.IsMatch(valueTrimmed, @"^[0-9]{9,14}$"))
                throw new InvalidREGONException();

            Value = valueTrimmed;
        }

        public static REGON? From(string? Value) => string.IsNullOrEmpty(Value) ? null : new REGON(Value);
    }

    public sealed class InvalidREGONException : AppException
    {
        public InvalidREGONException() : base("Numer REGON jest niepoprawny")
        {
        }
    }
}