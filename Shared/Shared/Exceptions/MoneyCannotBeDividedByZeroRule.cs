using Vero.Shared.DDD;

namespace Vero.Shared.Exceptions
{
    public sealed class MoneyCannotBeDividedByZeroRule(decimal divisor) : BusinessRule
    {
        public override string Message => "Nie można podzielić wartości przez 0";

        public override bool IsBroken()
        {
            return divisor < 0;
        }
    }
}