using Vero.Shared.DDD;

namespace Vero.Shared.ValueObjects
{
    public sealed record Price : ValueObject
    {
        public Price(Money net, Money gross, Money vat, VatRate vatRate)
        {
            Net = new Money(net);
            Gross = new Money(gross);
            Vat = new Money(vat);

            VatRate = vatRate;

            CheckRule(new PriceNetAndVatAndGrossShouldHaveTheSameCurrencyRule(Net, Vat, Gross));
            CheckRule(new GrossPriceMustBeGreaterOrEqualNetPriceRule(Net, Gross));
            //CheckRule(new PriceSumOfNetPriceAndVatPriceMustBeEqualGrossPriceRule(Gross, Vat, Net));
        }

        public Price(Money net, VatRate vatRate)
        {
            Net = new Money(net);
            Gross = net * (vatRate.Rate + 1);
            Vat = Gross - Net;

            VatRate = vatRate;
        }

        public Price(Price other) : base(other)
        {
            Net = new Money(other.Net);
            Gross = new Money(other.Gross);
            Vat = new Money(other.Vat);
            VatRate = other.VatRate;
        }

        private Price()
        {
        }

        public bool IsZero => Gross.Value == 0;

        public Currency Currency => Net.Currency;

        public VatRate VatRate { get; }

        public Money Net { get; }

        public Money Gross { get; }

        public Money Vat { get; }

        public static bool operator >(Price price, Price other) => price.Gross > other.Gross;

        public static bool operator >=(Price price, Price other) => price.Gross >= other.Gross;

        public static bool operator <(Price price, Price other) => price.Gross < other.Gross;

        public static bool operator <=(Price price, Price other) => price.Gross <= other.Gross;

        public static Price operator *(Price price, int multiplier) => FromGross(price.Gross * multiplier, price.VatRate);

        public static Price operator *(Price price, Amount amount) => price * amount.Value;

        public static Price operator -(Price price, Price other) => new(price.Net - other.Net, price.Gross - other.Gross, price.Vat - other.Vat, price.VatRate);

        public static Price operator +(Price price, Price other) => new(price.Net + other.Net, price.Gross + other.Gross, price.Vat + other.Vat, price.VatRate);

        public static Price Zero(Currency currency, VatRate vatRate) => FromGrossAmount(0, currency, vatRate);

        public static Price From(Price other) => new(other.Net, other.Gross, other.Vat, other.VatRate);

        public static Price FromNet(Money net, VatRate vatRate) => new(net, vatRate);

        public static Price FromGross(Money gross, VatRate vatRate) => new(gross * (1 / (1 + vatRate.Rate)), vatRate);

        public static Price FromGrossAmount(decimal grossAmount, Currency currency, VatRate vatRate) => FromGross(new Money(grossAmount, currency), vatRate);

        private sealed class GrossPriceMustBeGreaterOrEqualNetPriceRule(Money net, Money gross) : BusinessRule
        {
            public override string Message => "Wartość brutto musi byc wikększa lub równa wartości netto!";

            public override bool IsBroken() => net.Value >= 0 ? net > gross : net < gross;
        }

        private sealed class PriceSumOfNetPriceAndVatPriceMustBeEqualGrossPriceRule(Money gross, Money vat, Money net) : BusinessRule
        {
            public override string Message => "Suma wartości netto i Vat jest różna od wartości burtto!";

            public override bool IsBroken() => vat + net != gross;
        }

        private sealed class PriceNetAndVatAndGrossShouldHaveTheSameCurrencyRule(Money net, Money vat, Money gross) : BusinessRule
        {
            public override string Message => "Wartości brutto vat i netto muszą być tej samej waluty!";

            public override bool IsBroken() => net.Currency != vat.Currency || net.Currency != gross.Currency;
        }
    }
}