using Vero.Shared.DDD;

namespace Vero.Shared.ValueObjects
{
    public sealed record RecipientName : ValueObjectOf<string>
    {
        public RecipientName(string value) : base(value)
        {
            Value = value?.Trim() ?? "";//old value in db may not contains null
        }

        public static RecipientName Create(string value)
        {
            CheckRule(new RecipientNameMustNotBeEmptyRule(value));
            return new RecipientName(value);
        }
    }

    public sealed class RecipientNameMustNotBeEmptyRule : BusinessRule
    {
        private readonly string _value;

        internal RecipientNameMustNotBeEmptyRule(string value)
        {
            _value = value;
        }

        public override string Message => "Nazwa odbiorcy nie może być pusta!";

        public override bool IsBroken() => string.IsNullOrWhiteSpace(_value);
    }
}