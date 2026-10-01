using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Vero.Shared.EntityFramework.Converters
{
    public sealed class DateTimeOffsetToUniversalTimeConverter : ValueConverter<DateTimeOffset, DateTimeOffset>
    {
        public DateTimeOffsetToUniversalTimeConverter() : base(d => d.ToUniversalTime(), d => d.ToUniversalTime())
        {
        }
    }
}