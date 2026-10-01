using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Vero.Shared.ValueObjects;

namespace Vero.Shared.EntityFramework.Converters
{
    public static class EmailAddressConverters
    {
        public static PropertyBuilder<List<EmailAddress>> HasJsonConversion(this PropertyBuilder<List<EmailAddress>> propertyBuilder)
        {
            var serialize = (List<EmailAddress> obj) => JsonConvert.SerializeObject(obj.Select(r => r.Value));

            var deserialize = (string json) =>
            {
                var dbo = JsonConvert.DeserializeObject<List<string>>(json);
                return dbo?.Select(s => new EmailAddress(s))
                           .ToList() ??
                       new List<EmailAddress>();
            };

            var converter = new ValueConverter<List<EmailAddress>, string>(v => serialize(v), v => deserialize(v));
            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.HasColumnType("jsonb");

            return propertyBuilder;
        }
    }
}