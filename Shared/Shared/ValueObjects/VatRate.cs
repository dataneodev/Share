using Vero.Shared.DDD;

namespace Vero.Shared.ValueObjects
{
    public sealed class VatRate : Enumeration<VatRate>
    {
        public static readonly VatRate ZeroPercent = new VatRate(1, "0%", 0);
        public static readonly VatRate FivePercent = new VatRate(2, "5%", 0.05m);
        public static readonly VatRate EightPercent = new VatRate(3, "8%", 0.08m);
        public static readonly VatRate TwentyThreePercent = new VatRate(4, "23%", 0.23m);
        public static readonly VatRate VatExemption = new VatRate(5, "Z.w.", 0);

        public VatRate(int id, string value, decimal rate) : base(id, value)
        {
            Rate = rate;
        }

        public decimal Rate { get; }
    }
}