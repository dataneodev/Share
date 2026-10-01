using System.Globalization;

namespace Vero.Shared.Extensions
{
    public static class MoneyExtensions
    {
        public static string FormatMoneyRaw(decimal moneyValue, string currencyName, CultureInfo cultureInfo)
        {
            cultureInfo.NumberFormat.NumberGroupSeparator = " ";
            cultureInfo.NumberFormat.NumberDecimalSeparator = ",";

            return $@"{moneyValue.ToString("N2", cultureInfo)} {currencyName.ToUpper()}";
        }
    }
}