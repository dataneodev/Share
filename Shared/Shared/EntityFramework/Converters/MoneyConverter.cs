using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Vero.Shared.Extensions;
using Vero.Shared.ValueObjects;

namespace Vero.Shared.EntityFramework.Converters
{
    public static class MoneyConverter
    {
        public static string? Serialize(Money? obj, JsonSerializerSettings settings) =>
            obj is null ? null : JsonConvert.SerializeObject(new MoneyDbo { Value = obj.Value, CurrencyId = obj.Currency.Id }, settings);

        public static Money? Deserialize(string? json, JsonSerializerSettings settings)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            var dbo = JsonConvert.DeserializeObject<MoneyDbo>(json, settings);
            if (dbo is null)
                return null;

            var currency = Currency.FromId(dbo.CurrencyId);
            return new Money(dbo.Value, currency);
        }

        public static PropertyBuilder<Money> HasJsonConversion(this PropertyBuilder<Money> propertyBuilder) => propertyBuilder.HasMoneyConversion();

        public static PropertyBuilder<Money> HasMoneyConversion(this PropertyBuilder<Money> propertyBuilder)
        {
            var settings = JsonConfig.SerializerSettings();
            var converter = new ValueConverter<Money, string>(v => Serialize(v, settings)!, v => Deserialize(v, settings)!);

            var comparer = new ValueComparer<Money>((l, r) => l != null && r != null && l == r, v => v.GetHashCode(), v => new Money(v.Value, v.Currency));

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.Metadata.SetValueComparer(comparer);
            propertyBuilder.HasColumnType("jsonb");

            return propertyBuilder;
        }

        public static PropertyBuilder<Money?> HasNullableJsonConversion(this PropertyBuilder<Money?> propertyBuilder) =>
            propertyBuilder.HasNullableMoneyConversion();

        public static PropertyBuilder<Money?> HasNullableMoneyConversion(this PropertyBuilder<Money?> propertyBuilder)
        {
            var settings = JsonConfig.SerializerSettings();
            var converter = new ValueConverter<Money?, string?>(v => Serialize(v, settings), v => Deserialize(v, settings));
            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("jsonb");

            return propertyBuilder;
        }

        public static PropertyBuilder<MoneyAmount?> HasNullableMoneyAmountConversion(this PropertyBuilder<MoneyAmount?> propertyBuilder)
        {
            var converter = new ValueConverter<MoneyAmount?, decimal?>(
                o => o == null ? null : o.Value,
                v => v == null ? null : (MoneyAmount?)Activator.CreateInstance(typeof(MoneyAmount), v)
            );

            var comparer = new ValueComparer<MoneyAmount>((l, r) => l != null && r != null && l == r, v => v.GetHashCode(), v => new MoneyAmount(v.Value));

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.Metadata.SetValueComparer(comparer);

            return propertyBuilder;
        }

        public static PropertyBuilder<MoneyAmount> HasMoneyAmountConversion(this PropertyBuilder<MoneyAmount> propertyBuilder)
        {
            var converter = new ValueConverter<MoneyAmount, decimal?>(o => o.Value, v => (MoneyAmount)Activator.CreateInstance(typeof(MoneyAmount), v)!);

            var comparer = new ValueComparer<MoneyAmount>((l, r) => l != null && r != null && l == r, v => v.GetHashCode(), v => new MoneyAmount(v.Value));

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.Metadata.SetValueComparer(comparer);

            return propertyBuilder;
        }

        private sealed class MoneyDbo
        {
            public decimal Value { get; init; }

            public int CurrencyId { get; init; }
        }
    }
}