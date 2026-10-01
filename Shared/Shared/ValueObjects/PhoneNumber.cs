using System.Text.RegularExpressions;
using Vero.Shared.DDD;
using Vero.Shared.Exceptions;

namespace Vero.Shared.ValueObjects
{
    public sealed record PhoneNumber : ValueObjectOf<string>
    {
        private static readonly Regex _regex = new(@"^\+[1-9][0-9]{7,14}$");

        public PhoneNumber(string value) : base(value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new PhoneNumberEmptyException();

            value = value.Replace(" ", string.Empty);

            if (!_regex.IsMatch(value))
                throw new InvalidPhoneNumberFormatException(value);

            Value = value;
        }
    }

    internal sealed class InvalidPhoneNumberFormatException : AppException
    {
        public InvalidPhoneNumberFormatException(string phone) : base($"Niepoprawny format numeru telefonu: {phone}!")
        {
        }
    }

    internal sealed class PhoneNumberEmptyException : AppException
    {
        public PhoneNumberEmptyException() : base("Numer telefonu nie może być pusty!")
        {
        }
    }
}