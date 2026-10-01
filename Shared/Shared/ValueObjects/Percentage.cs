using Vero.Shared.DDD;
using Vero.Shared.Extensions;

namespace Vero.Shared.ValueObjects
{
    public sealed record Percentage(decimal Value) : ValueObjectOf<decimal>(Value.Round(2))
    {
    }
}