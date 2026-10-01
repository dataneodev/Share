using Vero.Shared.DDD;

namespace Vero.Shared.ValueObjects
{
    public sealed record Distance(double Value) : ValueObjectOf<double>(Value)
    {
        public string GetDistanceDesc() => $"{Value} km";
    }
}