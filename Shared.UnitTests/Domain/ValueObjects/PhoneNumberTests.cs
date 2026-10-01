using FluentAssertions;
using ProfiBiznes.Shared.Domain.Rules;
using ProfiBiznes.Shared.Domain.ValueObjects;
using ProfiBiznes.Shared.Infrastructure.Extensions;

namespace ProfiBiznes.Shared.UnitTests.Domain.ValueObjects
{
    public sealed class PhoneNumberTests
    {
        [Theory]
        [InlineData("+48 123 321 321")]
        [InlineData("+48123321321")]
        public void Constructor_InvalidPhoneNumber_IsCorrect(string num)
        {
            // Act
            var number = new PhoneNumber(num);

            //Assert
            number.Should()
                .NotBeNull();

            number.Value
                .Should()
                .MatchRegex(@"^\+\d+$");
        }

        [Theory]
        [InlineData("123321321")]
        [InlineData("123 321 321")]
        [InlineData("+48123321321d")]
        public void Constructor_InvalidPhoneNumber_Throws(string num)
        {
            // Act
            var act = () => new PhoneNumber(num);

            // Assert
            act.ShouldThrowBusinessRuleValidationException<PhoneNumberMustBeInCorrectFormatRule>();
        }
    }
}