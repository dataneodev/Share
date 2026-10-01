using Vero.Shared.DDD;

namespace Vero.Shared.ValueObjects
{
    public sealed record PropertyNumber : ValueObjectOf<string>
    {
        public PropertyNumber(string value) : base(value)
        {
        }

        public static PropertyNumber Create(string value)
        {
            CheckRule(new PropertyNumberCanNotBeEmptyRule(value));
            return new PropertyNumber(value);
        }

        public static PropertyNumber? CreateOrNull(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            CheckRule(new PropertyNumberCanNotBeEmptyRule(value));
            return new PropertyNumber(value);
        }
    }

    public sealed class PropertyNumberCanNotBeEmptyRule : BusinessRule
    {
        private readonly string _value;

        public PropertyNumberCanNotBeEmptyRule(string value)
        {
            _value = value;
        }

        public override string Message => "Numer posesji nie może być pusty!";

        public override bool IsBroken() => string.IsNullOrWhiteSpace(_value);
    }
}