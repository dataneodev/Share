using Vero.Shared.DDD;

namespace Vero.Shared.ValueObjects
{
    public sealed record GeoPosition(double Latitude, double Longitude) : ValueObject;
}