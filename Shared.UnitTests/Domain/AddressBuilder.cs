using Bogus;
using ProfiBiznes.Shared.Domain.Enumerations;
using ProfiBiznes.Shared.Domain.ValueObjects;

namespace ProfiBiznes.Shared.UnitTests.Domain
{
    public static class AddressBuilder
    {
        private static readonly Faker _faker = new();

        public static Address Create(
            string? street = null,
            string? buildingNumber = null,
            string? flatNumber = null,
            string? postCode = null,
            string? city = null
        ) =>
            new(
                street ?? _faker.Address.StreetName(),
                buildingNumber ?? _faker.Address.BuildingNumber(),
                flatNumber ??
                _faker.Random
                    .Int()
                    .ToString(),
                postCode ?? _faker.Address.ZipCode(),
                city ?? _faker.Address.City(),
                Country.PL
            );
    }
}