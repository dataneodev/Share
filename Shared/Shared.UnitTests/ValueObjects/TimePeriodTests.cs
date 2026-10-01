using FluentAssertions;
using Vero.Shared.ValueObjects;
using Xunit;

namespace Shared.UnitTests.ValueObjects
{	
    public sealed class TimePeriodTests
    {
		
        [Fact]
        public void TimePeriod_should_be_equal()
        {
            var timePeriod1 = new TimePeriod(new TimePeriodHours(1), new TimePeriodMinutes(30));
            var timePeriod2 = new TimePeriod(new TimePeriodHours(1), new TimePeriodMinutes(30));

            timePeriod1.Should()
                .Be(timePeriod2);
        }

        [Fact]
        public void TimePeriod_should_not_be_equal()
        {
            var timePeriod1 = new TimePeriod(new TimePeriodHours(1), new TimePeriodMinutes(30));
            var timePeriod2 = new TimePeriod(new TimePeriodHours(1), new TimePeriodMinutes(31));

            timePeriod1.Should()
                .NotBe(timePeriod2);
        }

        [Theory]
        [InlineData(1, 0, 60)]
        [InlineData(2, 0, 120)]
        [InlineData(1, 29, 89)]
        [InlineData(24, 0, 24 * 60)]
        public void TimePeriod_should_calculate_to_minutes(int hours, int minutes, int result)
        {
            var timePeriod = new TimePeriod(new TimePeriodHours(hours), new TimePeriodMinutes(minutes));
            var period = timePeriod.ToMinutes();

            period.Should()
                .Be(result);
        }

        [Fact]
        public void TimePeriod_should_throw_on_negative_minutes()
        {
            var testAction = () => new TimePeriod(new TimePeriodHours(1), new TimePeriodMinutes(-1));
            testAction.Should()
                .Throw<InvalidTimePeriodMinutesException>();
        }

        [Fact]
        public void TimePeriod_should_throw_on_minutes_greater_than_59()
        {
            var testAction = () => new TimePeriod(new TimePeriodHours(1), new TimePeriodMinutes(60));
            testAction.Should()
                .Throw<InvalidTimePeriodMinutesException>();
        }

        [Fact]
        public void TimePeriod_should_throw_on_negative_hours()
        {
            var testAction = () => new TimePeriod(new TimePeriodHours(-1), new TimePeriodMinutes(1));
            testAction.Should()
                .Throw<InvalidTimePeriodHoursException>();
        }

        [Fact]
        public void TimePeriod_should_NOT_throw_on_zero_hours()
        {
            var testAction = () => new TimePeriod(new TimePeriodHours(0), new TimePeriodMinutes(1));
            testAction.Should()
                .NotThrow<InvalidTimePeriodHoursException>();
        }

        [Fact]
        public void TimePeriod_should_NOT_throw_on_zero_minutes()
        {
            var testAction = () => new TimePeriod(new TimePeriodHours(1), new TimePeriodMinutes(0));
            testAction.Should()
                .NotThrow<InvalidTimePeriodMinutesException>();
        }

        [Fact]
        public void TimePeriod_should_NOT_throw_on_zero_hours_and_minutes()
        {
            var testAction = () => new TimePeriod(new TimePeriodHours(0), new TimePeriodMinutes(0));
            testAction.Should()
                .NotThrow<InvalidTimePeriodHoursException>();

            testAction.Should()
                .NotThrow<InvalidTimePeriodMinutesException>();
        }

        [Fact]
        public void TimePeriod_should_NOT_throw_on_23_hours_and_59_minutes()
        {
            var testAction = () => new TimePeriod(new TimePeriodHours(23), new TimePeriodMinutes(59));
            testAction.Should()
                .NotThrow<InvalidTimePeriodHoursException>();

            testAction.Should()
                .NotThrow<InvalidTimePeriodMinutesException>();
        }
    }
}