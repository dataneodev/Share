using Vero.Shared.DDD;

namespace Vero.Shared.ValueObjects
{
    public sealed record FlatNumber : ValueObjectOf<string?>
    {
        public FlatNumber(string? value) : base(value?.Trim())
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Value = null;
            }

            CheckRule(new FlatNumberMustNotBeTooLongRule(Value));
        }

        public static FlatNumber Create(string? value) => new(value);
    }

    public sealed class FlatNumberMustNotBeTooLongRule : BusinessRule
    {
        private const int MaxFlatNumberLength = 12;
        private readonly string? _value;

        internal FlatNumberMustNotBeTooLongRule(string? value)
        {
            _value = value;
        }

        public override string Message => "Numer lokalu jest za długi!";

        public override bool IsBroken() => _value?.Length > MaxFlatNumberLength;
    }
}