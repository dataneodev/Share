using Vero.Shared.DDD;

namespace Vero.Shared.ValueObjects
{
    public sealed record Address : ValueObject
    {
        public Address(
            RecipientName name,
            Country country,
            City city,
            PostCode postCode,
            Street street,
            PropertyNumber propertyNumber,
            FlatNumber flatNumber
        )
        {
            Name = name;
            Country = country;
            City = city;
            PostCode = postCode;
            Street = street;
            PropertyNumber = propertyNumber;
            FlatNumber = flatNumber;
        }

        public Address(Address other) : base(other)
        {
            Name = other.Name;
            Country = other.Country;
            City = other.City;
            PostCode = other.PostCode;
            Street = other.Street;
            PropertyNumber = other.PropertyNumber;
            FlatNumber = other.FlatNumber;
        }

        private Address()
        {
        }

        public RecipientName Name { get; }

        public Country Country { get; }

        public City City { get; }

        public PostCode PostCode { get; }

        public Street Street { get; }

        public PropertyNumber PropertyNumber { get; }

        public FlatNumber FlatNumber { get; }
    }
}