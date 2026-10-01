using System.Text.RegularExpressions;
using Vero.Shared.DDD;
using Vero.Shared.Exceptions;

namespace Vero.Shared.ValueObjects
{
    public sealed record Nip : ValueObject
    {
        public const int MinNipNumberLength = 2;

        private Nip()
        {
        }

        public Nip(Country country, string number)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new NipValueCannotBeEmptyException();

            if (number.StartsWith(country.Value, StringComparison.InvariantCultureIgnoreCase))
                number = number.Substring(country.Value.Length);

            number = new Regex(@"[^0-9A-Za-z]").Replace(number, string.Empty);

            if (number.Length < MinNipNumberLength)
                throw new NipValueCannotBeEmptyException();

            Country = country;
            Number = number.Trim();
        }

        public Nip(Nip other) : base(other)
        {
            Country = other.Country;
            Number = other.Number;
        }

        public Country Country { get; }

        public string Number { get; }

        public string GetNumerWithCountryCode()
        {
            return $"{Country.Value}{Number}";
        }

        public static Nip From(int CountryId, string Number)
        {
            return new Nip(Country.FromId(CountryId), Number);
        }
    }

    internal sealed class NipValueCannotBeEmptyException : AppException
    {
        public NipValueCannotBeEmptyException() : base("Numer NIP nie może być pusty!")
        {
        }
    }
}