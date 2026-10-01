using Vero.Shared.DDD;

namespace Vero.Shared.ValueObjects
{
    public sealed class GTUCode : Enumeration<GTUCode>
    {
        public static readonly GTUCode GTU1 = new GTUCode(1, "GTU 1");
        public static readonly GTUCode GTU2 = new GTUCode(2, "GTU 2");
        public static readonly GTUCode GTU3 = new GTUCode(3, "GTU 3");
        public static readonly GTUCode GTU4 = new GTUCode(4, "GTU 4");
        public static readonly GTUCode GTU5 = new GTUCode(5, "GTU 5");
        public static readonly GTUCode GTU6 = new GTUCode(6, "GTU 6");
        public static readonly GTUCode GTU7 = new GTUCode(7, "GTU 7");

        public GTUCode(int id, string value) : base(id, value)
        {
        }
    }
}