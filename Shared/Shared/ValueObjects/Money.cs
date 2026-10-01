using Vero.Shared.DDD;
using Vero.Shared.Exceptions;

namespace Vero.Shared.ValueObjects
{
    public sealed record Money : ValueObject
    {
        public Money(decimal value, Currency currency)
        {
            Value = Round(value, currency);
            Currency = currency;
        }

        public Money(Money other) : base(other)
        {
            Currency = other.Currency;
            Value = other.Value;
        }

        private Money()
        {
        }

        public decimal Value { get; }

        public Currency Currency { get; }

        public bool IsZero => Value == 0;

        public bool Equals(Money? other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return base.Equals(other) && Value == other.Value && Currency == other.Currency;
        }

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Value, Currency);

        public bool EqualsCurrency(Money? money) => money?.Currency == Currency;

        public static Money Zero(Currency currency) => new(0, currency);

        public static bool operator >(Money v1, Money v2)
        {
            CheckRule(new MathOnMoneyCanBePerformedOnlyWhenCurrenciesMatchRule(v1, v2));

            return v1.Value > v2.Value;
        }

        public static bool operator >=(Money v1, Money v2)
        {
            CheckRule(new MathOnMoneyCanBePerformedOnlyWhenCurrenciesMatchRule(v1, v2));

            return v1.Value >= v2.Value;
        }

        public static bool operator <(Money v1, Money v2)
        {
            CheckRule(new MathOnMoneyCanBePerformedOnlyWhenCurrenciesMatchRule(v1, v2));

            return v1.Value < v2.Value;
        }

        public static bool operator <=(Money v1, Money v2)
        {
            CheckRule(new MathOnMoneyCanBePerformedOnlyWhenCurrenciesMatchRule(v1, v2));

            return v1.Value <= v2.Value;
        }

        public static Money operator +(Money v1, Money v2)
        {
            CheckRule(new MathOnMoneyCanBePerformedOnlyWhenCurrenciesMatchRule(v1, v2));

            return new Money(decimal.Add(v1.Value, v2.Value), v1.Currency);
        }

        public static Money operator -(Money v1, Money v2)
        {
            CheckRule(new MathOnMoneyCanBePerformedOnlyWhenCurrenciesMatchRule(v1, v2));

            return new Money(decimal.Subtract(v1.Value, v2.Value), v1.Currency);
        }

        public static Money operator *(Money money, int number) => new(money.Value * number, money.Currency);

        public static Money operator *(Money money, decimal factor) => new(decimal.Multiply(money.Value, factor), money.Currency);

        public static Money operator *(Money a, Percentage percentage) => new(decimal.Multiply(a.Value, percentage.Value), a.Currency);

        public static Money operator *(Money a, Amount amount) => new(decimal.Multiply(a.Value, amount.Value), a.Currency);

        public override string ToString() => $"{Value}{Currency.Value}";

        private static decimal Round(decimal amount, Currency currency) => Math.Round(amount, currency.Precision, MidpointRounding.AwayFromZero);

        private sealed class ExchangeRateCannotBeZeroException() : AppException("Kurs wymiany waluty nie może być równy zero!");

        private sealed class CanNotCompareDifferentCurrenciesException(Money? v1, Money? v2) : AppException($"Nie można porównać różnych walut {v1} {v2}");

        private sealed class MathOnMoneyCanBePerformedOnlyWhenCurrenciesMatchRule(Money v1, Money v2) : BusinessRule
        {
            public override string Message => $"Nie można porównać różnych walut {v1.Currency} {v2.Currency}";

            public override bool IsBroken() => v1.Currency != v2.Currency;
        }
    }
}