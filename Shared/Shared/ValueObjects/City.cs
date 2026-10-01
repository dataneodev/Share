using Vero.Shared.DDD;

namespace Vero.Shared.ValueObjects
{
    public sealed record City : ValueObjectOf<string>
    {
        public City(string value) : base(value)
        {
            CheckRule(new CityCanNotBeEmptyRule(value));

            Value = value.Trim();

            CheckRule(new CityMaximumLengthExceededRule(Value));
        }
    }

    public sealed class CityCanNotBeEmptyRule : BusinessRule
    {
        private readonly string _value;

        public CityCanNotBeEmptyRule(string value)
        {
            _value = value;
        }

        public override string Message => "Nazwa miasta nie może być pusta!";

        public override bool IsBroken() => string.IsNullOrWhiteSpace(_value);
    }

    public sealed class CityMaximumLengthExceededRule : BusinessRule
    {
        private const int MaxCityLength = 120;
        private readonly string _value;

        public CityMaximumLengthExceededRule(string value)
        {
            _value = value;
        }

        public override string Message => $"Miasto nie może posiadać więcej niż {MaxCityLength} znaków!";

        public override bool IsBroken() => (_value?.Length ?? 0) > MaxCityLength;
    }
}