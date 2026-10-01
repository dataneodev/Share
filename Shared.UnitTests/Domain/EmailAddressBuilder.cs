using Bogus;
using ProfiBiznes.Shared.Domain.ValueObjects;

namespace ProfiBiznes.Shared.UnitTests.Domain
{
    public static class EmailAddressBuilder
    {
        private static readonly Faker _faker = new();

        public static EmailAddress Create(string? uniqueSuffix = null) => new(_faker.Internet.Email(uniqueSuffix: uniqueSuffix, provider: "vanto.pl"));
    }
}