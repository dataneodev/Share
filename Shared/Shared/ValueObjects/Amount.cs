using Vero.Shared.DDD;

namespace Vero.Shared.ValueObjects
{
    public sealed record Amount(int Value) : ValueObjectOf<int>(Value);
}