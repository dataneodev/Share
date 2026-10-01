using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vero.Shared.EntityFramework.Converters;
using Vero.Shared.ValueObjects;

namespace Vero.Shared.EntityFramework
{
    public static class OwnsOneModelBuilderExtensions
    {
        public static void NipProperty<T>(this OwnedNavigationBuilder<T, Nip> b, string numberColumnName, string countryColumnName)
            where T : class
        {
            b.Property(v => v.Number)
                .IsRequired()
                .HasColumnName(numberColumnName);

            b.Property(v => v.Country)
                .IsRequired()
                .HasEnumerationConversion()
                .HasColumnName(countryColumnName);
        }

        public static void AddressProperty<T>(this OwnedNavigationBuilder<T, Address> b, string columnNamePrefix)
            where T : class
        {
            b.Property(v => v.Name)
                .IsRequired()
                .HasStringConversion()
                .HasColumnName(FormatColumnName(columnNamePrefix, "name"));

            b.Property(v => v.Country)
                .IsRequired()
                .HasEnumerationConversion()
                .HasColumnName(FormatColumnName(columnNamePrefix, "country_id"));

            b.Property(v => v.City)
                .IsRequired()
                .HasStringConversion()
                .HasColumnName(FormatColumnName(columnNamePrefix, "city"));

            b.Property(v => v.Street)
                .IsRequired()
                .HasStringConversion()
                .HasColumnName(FormatColumnName(columnNamePrefix, "street"));

            b.Property(v => v.PostCode)
                .IsRequired()
                .HasStringConversion()
                .HasColumnName(FormatColumnName(columnNamePrefix, "post_code"));

            b.Property(v => v.PropertyNumber)
                .IsRequired()
                .HasStringConversion()
                .HasColumnName(FormatColumnName(columnNamePrefix, "property_number"));

            b.Property(v => v.FlatNumber)
                .IsRequired(false)
                .HasStringNullableConversion()
                .HasColumnName(FormatColumnName(columnNamePrefix, "flat_number"));
        }

        private static string FormatColumnName(string? prefix, string columnName) => string.IsNullOrWhiteSpace(prefix) ? columnName : $"{prefix}_{columnName}";

        public static void MoneyProperty<T>(this OwnedNavigationBuilder<T, Money> b, string valueColumnName, string currencyColumnName)
            where T : class
        {
            b.Property(v => v.Value)
                .IsRequired()
                .HasColumnName(valueColumnName);

            b.Property(v => v.Currency)
                .IsRequired()
                .HasEnumerationConversion()
                .HasColumnName(currencyColumnName);
        }

        public static void PriceProperty<T>(this OwnedNavigationBuilder<T, Price> b, string prefix)
            where T : class
        {
            b.OwnsOne(v => v.Net, m => m.MoneyProperty(FormatColumnName(prefix, "net_price"), FormatColumnName(prefix, "net_price_currency_id")));
            b.OwnsOne(v => v.Gross, m => m.MoneyProperty(FormatColumnName(prefix, "gross_price"), FormatColumnName(prefix, "gross_price_currency_id")));
            b.OwnsOne(v => v.Vat, m => m.MoneyProperty(FormatColumnName(prefix, "vat_price"), FormatColumnName(prefix, "vat_price_currency_id")));
            b.Property(p => p.VatRate)
                .HasEnumerationConversion()
                .HasColumnName(FormatColumnName(prefix, "vat_rate_id"));
        }
    }
}