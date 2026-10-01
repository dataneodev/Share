using System.Text.RegularExpressions;
using Vero.Shared.DDD;

namespace Vero.Shared.ValueObjects
{
    public sealed record CleanPostCode : ValueObjectOf<string?>
    {
        private readonly Regex _regex = new(@"[^0-9a-zA-Z]+");

        public CleanPostCode(string? value) : base(value)
        {
            Value = value?.ToUpper()
                .Trim();

            if (!string.IsNullOrWhiteSpace(Value))
            {
                Value = _regex.Replace(Value, string.Empty);
            }

            if (string.IsNullOrWhiteSpace(Value))
            {
                Value = null;
            }
        }

        public bool IsParentOf(CleanPostCode postCode) => postCode.Value is not null && Value is not null && postCode.Value.StartsWith(Value);

        public bool IsChildOf(CleanPostCode postCode) => postCode.IsParentOf(this);
    }
}