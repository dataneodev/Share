using Vero.Shared.DDD;

#pragma warning disable CS8618// Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Vero.Shared.ValueObjects
{
    public sealed record Markup : ValueObject
    {
        private Markup(Percentage percentage, MoneyAmount markupValue)
        {
            Percentage = percentage;
            MarkupValue = markupValue;
        }

        private Markup()
        {
        }

        public Percentage Percentage { get; }
        public MoneyAmount MarkupValue { get; }

        public bool Equals(Margin? other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return base.Equals(other);
        }

        public static Markup? From(MoneyAmount? basePrice, MoneyAmount? salePrice)
        {
            if (basePrice is null || salePrice is null)
                return null;

            var diff = (salePrice - basePrice) / basePrice;
            var percentage = new Percentage(diff.Value * 100);

            return new Markup(percentage, diff);
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}