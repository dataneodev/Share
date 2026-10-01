using Vero.Shared.DDD;

namespace Vero.Shared.ValueObjects
{
    public sealed record Street : ValueObjectOf<string>
    {
        public Street(string value) : base(value)
        {
            Value = value;
        }

        public static Street Create(string? value)
        {
            CheckRule(new StreetCanNotBeEmptyRule(value));
            CheckRule(new StreetMaximumLengthExceededRule(value));
            return new Street(value!);
        }

        public static Street? CreateOrNull(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            CheckRule(new StreetCanNotBeEmptyRule(value));
            CheckRule(new StreetMaximumLengthExceededRule(value));
            return new Street(value);
        }
    }

    public sealed class StreetCanNotBeEmptyRule : BusinessRule
    {
        private readonly string? _value;

        public StreetCanNotBeEmptyRule(string? value)
        {
            _value = value;
        }

        public override string Message => "Nazwa ulicy nie może być pusta!";

        public override bool IsBroken() => string.IsNullOrWhiteSpace(_value);
    }

    public sealed class StreetMaximumLengthExceededRule : BusinessRule
    {
        private const int MaxStreetLength = 120;
        private readonly string? _value;

        public StreetMaximumLengthExceededRule(string? value)
        {
            _value = value;
        }

        public override string Message => $"Nazwa ulicy nie może posiadać więcej niż {MaxStreetLength} znaków!";

        public override bool IsBroken() => (_value?.Length ?? 0) > MaxStreetLength;
    }
}