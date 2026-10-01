using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Vero.Shared.Extensions;
using Vero.Shared.ValueObjects;

namespace Vero.Shared.EntityFramework.Converters
{
    public static class VatRateConverter
    {
        public static PropertyBuilder<VatRate?> HasJsonConversion(this PropertyBuilder<VatRate?> propertyBuilder)
        {
            var setting = JsonConfig.SerializerSettings();

            var converter = new ValueConverter<VatRate?, string?>(
                v => v != null ? Serialize(v, setting) : null,
                v => v != null ? Deserialize(v, setting) : null
            );

            var comparer = new ValueComparer<VatRate>(
                (l, r) => l != null && r != null && l.Equals(r),
                v => v.GetHashCode(),
                v => new VatRate(v.Id, v.Value, v.Rate)
            );

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.Metadata.SetValueComparer(comparer);
            propertyBuilder.HasColumnType("jsonb");

            return propertyBuilder;
        }

        public static string? Serialize(VatRate? obj, JsonSerializerSettings settings)
        {
            if (obj is null)
                return null;

            return JsonConvert.SerializeObject(new VatRateDbo { Id = obj.Id, Value = obj.Value, Rate = obj.Rate }, settings);
        }

        public static VatRate? Deserialize(string json, JsonSerializerSettings settings)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            var dbo = JsonConvert.DeserializeObject<VatRateDbo>(json, settings);

            if (dbo is null)
                return null;

            return new VatRate(dbo.Id, dbo.Value, dbo.Rate);
        }

        private sealed class VatRateDbo
        {
            public int Id { get; init; }

            public string Value { get; init; }

            public decimal Rate { get; init; }
        }
    }
}