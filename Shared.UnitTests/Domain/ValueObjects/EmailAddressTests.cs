using FluentAssertions;
using ProfiBiznes.Shared.Domain.Exceptions;
using ProfiBiznes.Shared.Domain.ValueObjects;

namespace ProfiBiznes.Shared.UnitTests.Domain.ValueObjects
{
    public sealed class EmailAddressTests
    {
        [Theory]
        [InlineData("email")]
        [InlineData("email@example@example.com")]
        [InlineData("#@%^%#$@#$@#.com")]
        public void Constructor_InvalidEmail_Throws(string email)
        {
            // Act
            var act = () => new EmailAddress(email);

            // Assert
            act.Should()
                .Throw<IncorrectEmailAddressFormatException>();
        }
    }
}