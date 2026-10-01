using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Vero.Shared.Extensions;
using Vero.Shared.ValueObjects;

namespace Vero.Shared.EntityFramework.Converters
{
    public static class AddressConverters
    {
        private sealed class AddressDBO
        {
            public int CountryId { get; init; }

            public string Name { get; init; }

            public string City { get; init; }

            public string PostCode { get; init; }

            public string Street { get; init; }

            public string PropertyNumber { get; init; }

            public string? FlatNumber { get; init; }
        }

        public static PropertyBuilder<Address> HasJsonConversion(this PropertyBuilder<Address> propertyBuilder)
        {
            var settings = JsonConfig.SerializerSettings();
            var serialize = (Address obj) => JsonConvert.SerializeObject(
                new AddressDBO
                {
                    CountryId = obj.Country.Id,
                    City = obj.City.Value,
                    Name = obj.Name.Value,
                    PostCode = obj.PostCode.Value,
                    Street = obj.Street.Value,
                    PropertyNumber = obj.PropertyNumber.Value,
                    FlatNumber = obj.FlatNumber?.Value
                },
                settings
            );

            var deserialize = (string json) =>
            {
                var dbo = JsonConvert.DeserializeObject<AddressDBO>(json, settings)!;
                return new Address(
                    new RecipientName(dbo.Name),
                    Country.FromId(dbo.CountryId),
                    new City(dbo.City),
                    new PostCode(dbo.PostCode),
                    new Street(dbo.Street),
                    new PropertyNumber(dbo.PropertyNumber),
                    FlatNumber.Create(dbo.FlatNumber)
                );
            };

            var converter = new ValueConverter<Address, string>(v => serialize(v), v => deserialize(v));
            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("jsonb");

            return propertyBuilder;
        }

        public static PropertyBuilder<Address?> HasNullableJsonConversion(this PropertyBuilder<Address?> propertyBuilder)
        {
            var settings = JsonConfig.SerializerSettings();
            var serialize = (Address? obj) => obj is null
                ? null
                : JsonConvert.SerializeObject(
                    new AddressDBO
                    {
                        CountryId = obj.Country.Id,
                        City = obj.City.Value,
                        Name = obj.Name.Value,
                        PostCode = obj.PostCode.Value,
                        Street = obj.Street.Value,
                        PropertyNumber = obj.PropertyNumber.Value,
                        FlatNumber = obj.FlatNumber?.Value
                    },
                    settings
                );

            var deserialize = (string? json) =>
            {
                if (string.IsNullOrWhiteSpace(json))
                    return null;

                var dbo = JsonConvert.DeserializeObject<AddressDBO>(json, settings)!;
                return new Address(
                    new RecipientName(dbo.Name),
                    Country.FromId(dbo.CountryId),
                    new City(dbo.City),
                    new PostCode(dbo.PostCode),
                    new Street(dbo.Street),
                    new PropertyNumber(dbo.PropertyNumber),
                    new FlatNumber(dbo.FlatNumber)
                );
            };

            var converter = new ValueConverter<Address?, string?>(v => serialize(v), v => deserialize(v));

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("jsonb");

            return propertyBuilder;
        }
    }
}