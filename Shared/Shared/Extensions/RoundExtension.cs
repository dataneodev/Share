namespace Vero.Shared.Extensions
{
    public static class RoundExtension
    {
        public static decimal Round(this decimal value, int precision = 5) => Math.Round(value, precision, MidpointRounding.AwayFromZero);

        public static double Round(this float value, int precision = 5) => Math.Round(value, precision, MidpointRounding.AwayFromZero);
    }
}