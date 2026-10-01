using Bogus;
using ProfiBiznes.Shared.Domain.ValueObjects;

namespace ProfiBiznes.Shared.UnitTests.Domain
{
    public static class PhoneNumberBuilder
    {
        private static readonly Faker _faker = new();

        public static PhoneNumber Create(string? value = null) => new(value ?? _faker.Phone.PhoneNumber("+48 ### ### ###"));
    }
}