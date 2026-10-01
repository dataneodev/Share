using Vero.Shared.DDD;
using Vero.Shared.Exceptions;

#pragma warning disable CS8618// Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Vero.Shared.ValueObjects
{
    public sealed record Margin : ValueObject
    {
        private Margin(Percentage percentage, MoneyAmount marginValue)
        {
            Percentage = percentage;
            MarginValue = marginValue;
        }

        private Margin()
        {
        }

        public Percentage Percentage { get; }
        public MoneyAmount MarginValue { get; }

        public bool Equals(Margin? other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return base.Equals(other);
        }

        public static Margin? From(MoneyAmount? basePrice, MoneyAmount? salePrice)
        {
            if (basePrice is null || salePrice is null)
                return null;

            var diff = (salePrice - basePrice) / salePrice;
            var percentage = new Percentage(diff.Value * 100);

            return new Margin(percentage, diff);
        }

        public static Margin Create(MoneyAmount basePrice, MoneyAmount salePrice)
        {
            if (basePrice is null || salePrice is null)
                throw new NotFoundParametersToCalculateMargin();

            var diff = (salePrice - basePrice) / salePrice;
            var percentage = new Percentage(diff.Value * 100);

            return new Margin(percentage, diff);
        }

        public override int GetHashCode() => base.GetHashCode();

        public sealed class NotFoundParametersToCalculateMargin : AppException
        {
            public NotFoundParametersToCalculateMargin() : base("Nie podano właściwych parametrów do obliczenia marży!")
            {
            }
        }
    }
}