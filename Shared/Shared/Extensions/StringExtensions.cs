using ExCSS;
using System.Text;
using System.Text.RegularExpressions;
using Vero.Shared.ValueObjects;

namespace Vero.Shared.Extensions
{
    public static class Extensions
    {
        public static string GetMartenSearch(this string? search, int timeOffset = 0)
        {
            if (string.IsNullOrEmpty(search))
                return "";

            var trimedSearch = search.Trim();

            trimedSearch = ReplaceDate(trimedSearch, timeOffset);
            trimedSearch = ReplaceValue(trimedSearch);
            trimedSearch = ReplaceCurrency(trimedSearch);

            return trimedSearch.Trim();
        }

        public static string ToSnakeCase(this string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (text.Length < 2)
            {
                return text;
            }

            var sb = new StringBuilder();
            sb.Append(char.ToLowerInvariant(text[0]));
            for (var i = 1; i < text.Length; ++i)
            {
                var c = text[i];
                if (char.IsUpper(c))
                {
                    sb.Append('_');
                    sb.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        public static bool Has<T>(this IEnumerable<T> enumerable, params T[] items)
        {
            foreach (var item in items)
            {
                if (!enumerable.Contains(item))
                    return false;
            }

            return true;
        }

        private static string ReplaceValue(string search)
        {
            var match = CheckIfDateMatch(search.Replace("-", "."));
            if (match?.Success ?? false)
                return search;

            var regex = new Regex(@"(\d+)\s+(?=\d)");

            var result = regex.Replace(search, "$1").Replace(",", ".");

            var decimalRegex = new Regex(@"\d+\.\d{2}");

            return decimalRegex.Replace(
            result,
            match =>
            {
                var value = match.Value;

                if (value[value.Length - 1] == '0')
                {
                    value = value.Substring(0, value.Length - 1);
                }

                return value;
            });
        }

        private static string ReplaceCurrency(string search)
        {
            var currencies = Currency.GetAll();

            foreach (var currency in currencies)
            {
                var regex = new Regex($@"\b\d+(?:[\s]\d+)*(?:[\.,]\d+)?\s*{currency.Value}\b");
                var match = regex.Match(search);

                if (match.Success)
                {
                    var numberWithoutCurrency = match.Value.Replace(currency.Value, "").Trim();

                    return search.Replace(match.Value, numberWithoutCurrency);
                }
            }

            return search;
        }

        private static string ReplaceDate(string search, int offset)
        {
            var newSearch = AdjustDateTimeToAppTimeZone(search, offset);
            var match = CheckIfDateMatch(newSearch);

            if (match?.Success ?? false)
            {
                string newDate;
                if (match.Groups["hour"].Success && match.Groups["minute"].Success)
                {
                    newDate = $"{match.Groups["year"]}-{match.Groups["month"]}-{match.Groups["day"]}T{match.Groups["hour"]}:{match.Groups["minute"]}";
                }
                else if (match.Groups["year"].Success)
                {
                    newDate = $"{match.Groups["year"]}-{match.Groups["month"]}-{match.Groups["day"]}";
                }
                else
                {
                    newDate = $"{match.Groups["month"]}-{match.Groups["day"]}";
                }

                return newDate;
            }

            return search;
        }

        private static Match? CheckIfDateMatch(string search)
        {
            var patterns = new[]
           {
                new { Regex = @"(?'day'[0-9]{2})\.(?'month'[0-9]{2})\.(?'year'[0-9]{4}) (?'hour'[0-9]{2}):(?'minute'[0-9]{2})" },
                new { Regex = @"(?'day'[0-9]{2})\.(?'month'[0-9]{2})\.(?'year'[0-9]{4})" },
                new { Regex = @"(?'month'[0-9]{2})\.(?'year'[0-9]{4})" },
                new { Regex = @"(?'day'[0-9]{2})\.(?'month'[0-9]{2})" }
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(search, pattern.Regex);
                if (match.Success)

                    return match;
            };

            return null;
        }

        private static string AdjustDateTimeToAppTimeZone(string input, int offset)
        {
            if (DateTime.TryParseExact(input, "dd.MM.yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDateTime))
            {
                var localDateTimeOffset = new DateTimeOffset(parsedDateTime, TimeSpan.Zero).AddMinutes(-offset);

                return localDateTimeOffset.ToString("dd.MM.yyyy HH:mm");
            }

            return input;
        }
    }
}