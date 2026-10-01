using System.Globalization;
using Vero.Shared.DDD;

namespace Vero.Shared.ValueObjects
{
    public sealed class AppLanguage : Enumeration<AppLanguage>
    {
        public static readonly AppLanguage Polish = new(1, "pl");
        public static readonly AppLanguage English = new(2, "en");
        public static readonly AppLanguage German = new(3, "de");

        private AppLanguage(int id, string culture) : base(id, culture)
        {
            Culture = new CultureInfo(culture);
        }

        public CultureInfo Culture { get; }

        public static AppLanguage GetFallback() => English;

        public static AppLanguage? TryParse(string? culture) => GetAll()
            .FirstOrDefault(l => l.Culture.Name == culture);

        public void ApplyToCurrentThread()
        {
            if (!Equals(Thread.CurrentThread.CurrentUICulture, Culture))
                Thread.CurrentThread.CurrentUICulture = Culture;
        }
    }
}