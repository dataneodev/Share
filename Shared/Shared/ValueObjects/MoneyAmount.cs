using Vero.Shared.DDD;
using Vero.Shared.Exceptions;
using Vero.Shared.Extensions;

namespace Vero.Shared.ValueObjects
{
    public sealed record MoneyAmount(decimal Value) : ValueObjectOf<decimal>(Value.Round(2))
    {
        public bool Equals(MoneyAmount? other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return base.Equals(other) && Value.Round() == other.Value.Round();
        }

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Value);

        public MoneyAmount CalculateGrossPrice(VatRate vat) => new(Value * (1 + vat.Rate));

        public MoneyAmount CalculateVatPrice(VatRate vat) => new(Value * (1 + vat.Rate) - Value);

        public MoneyAmount MultiplyBy(decimal multiplyBy) => new(Value * multiplyBy);

        public static MoneyAmount operator +(MoneyAmount a, MoneyAmount b) => new(a.Value + b.Value);

        public static MoneyAmount operator /(MoneyAmount a, decimal divisor)
        {
            CheckRule(new MoneyCannotBeDividedByZeroRule(divisor));

            return new MoneyAmount(a.Value / divisor);
        }

        public static MoneyAmount operator *(MoneyAmount a, decimal factor) => new(a.Value * factor);

        public static MoneyAmount operator -(MoneyAmount a, MoneyAmount b) => new(a.Value - b.Value);

        public static MoneyAmount CreateWithRound(decimal value) => new(value.Round(2));
    }
}