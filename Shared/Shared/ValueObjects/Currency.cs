using Vero.Shared.DDD;

namespace Vero.Shared.ValueObjects
{
    public sealed class Currency : Enumeration<Currency>
    {
        public static readonly Currency PLN = new(1, "PLN", 2);
        public static readonly Currency EUR = new(2, "EUR", 2);
        public static readonly Currency GBP = new(3, "GBP", 2);
        public static readonly Currency CHF = new(4, "CHF", 2);
        public static readonly Currency USD = new(5, "USD", 2);
        public static readonly Currency ALL = new(6, "ALL", 2);
        public static readonly Currency AMD = new(7, "AMD", 2);
        public static readonly Currency AZN = new(8, "AZN", 2);
        public static readonly Currency BYN = new(9, "BYN", 2);
        public static readonly Currency BAM = new(10, "BAM", 2);
        public static readonly Currency BGN = new(11, "BGN", 2);
        public static readonly Currency CZK = new(13, "CZK", 2);
        public static readonly Currency DKK = new(14, "DKK", 2);
        public static readonly Currency GEL = new(15, "GEL", 2);
        public static readonly Currency HUF = new(16, "HUF", 2);
        public static readonly Currency ISK = new(17, "ISK", 2);
        public static readonly Currency MDL = new(18, "MDL", 2);
        public static readonly Currency MKD = new(19, "MKD", 2);
        public static readonly Currency NOK = new(20, "NOK", 2);
        public static readonly Currency RON = new(21, "RON", 2);
        public static readonly Currency RUB = new(22, "RUB", 2);
        public static readonly Currency RSD = new(23, "RSD", 2);
        public static readonly Currency SEK = new(24, "SEK", 2);
        public static readonly Currency TRY = new(25, "TRY", 2);
        public static readonly Currency UAH = new(26, "UAH", 2);

        //public static readonly Currency HRK = new Currency(12, "HRK"); -> kuna chorwacka przeszli tam na euro

        private Currency(int id, string value, int precision) : base(id, value)
        {
            Precision = precision;
        }

        public int Precision { get; }
    }
}