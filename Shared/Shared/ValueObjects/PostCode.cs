using System.Text.RegularExpressions;
using Vero.Shared.DDD;

namespace Vero.Shared.ValueObjects
{
    public sealed record PostCode : ValueObjectOf<string>
    {
        public const int MaxPostCodeLength = 40;

        private readonly Regex _regex = new(@"[^0-9a-zA-Z- ]+");

        public PostCode(string value) : base(value)
        {
            Value = _regex.Replace(value, string.Empty)
                .ToUpper();

            CheckRule(new PostCodeCanNotBeEmptyRule(Value));
            CheckRule(new PostCodeMaximumLengthExceededRule(Value));
        }

        public bool IsParentOf(PostCode postCode) => postCode.Value.StartsWith(Value);

        public bool IsChildOf(PostCode postCode) => postCode.IsParentOf(this);
    }

    public sealed class PostCodeCanNotBeEmptyRule : BusinessRule
    {
        private readonly string _value;

        public PostCodeCanNotBeEmptyRule(string value)
        {
            _value = value;
        }

        public override string Message => "Kod pocztowy nie może być pusty!";

        public override bool IsBroken() => string.IsNullOrWhiteSpace(_value);
    }

    public sealed class PostCodeMaximumLengthExceededRule : BusinessRule
    {
        private const int MaxPostCodeLength = 40;
        private readonly string _value;

        public PostCodeMaximumLengthExceededRule(string value)
        {
            _value = value;
        }

        public override string Message => $"Kod pocztowy nie może posiadać więcej niż {MaxPostCodeLength} znaków!";

        public override bool IsBroken() => (_value?.Length ?? 0) > MaxPostCodeLength;
    }
}