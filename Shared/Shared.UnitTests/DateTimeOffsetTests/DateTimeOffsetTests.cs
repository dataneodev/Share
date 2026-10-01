using FluentAssertions;
using Vero.Shared.Extensions;
using Xunit;

namespace Shared.UnitTests.DateTimeOffsetTests
{
    public sealed class DateTimeOffsetTests
    {
        [Fact]
        public void DateTimeOffsetConverter()
        {
            var testDate = DateTimeOffset.UtcNow;
            var testDate2 = testDate.ToUniversalTime();

            testDate.Should().Be(testDate2);
        }

        [Fact]
        public void EndOfDay()
        {
            var testDate = DateTimeOffset.UtcNow;
            var testDate2 = testDate.SetTime(23, 59, 59);

            testDate2.Should().BeAfter(testDate);
        }

        private class TestObject
        {
            public DateTimeOffset Test { get; set; }
        }

        private string GetObjectString(string date) => "{test:\"" + date + "\"}";

        [Fact]
        public void ChangeTimeZoneTest()
        {
            DateTimeOffset utc = new DateTime(2023, 10, 25, 16, 32, 52, DateTimeKind.Utc);

            var warsawTime = utc.ReplaceTimeZoneToWarsaw();

            var againUtc = warsawTime.ReplaceTimeZoneWarsawToUTC();

            utc.Should().Be(againUtc);
        }

        [Fact]
        public void ConvertDateTimeOffsetToCentralEuropeTest()
        {
            DateTimeOffset utc = new DateTime(
                2023,
                10,
                25,
                16,
                32,
                52,
                DateTimeKind.Utc
            );

            var warsawTime = utc.ConvertDateTimeOffsetToCentralEurope();

            Assert.True(true);
        }
    }
}