using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Vero.Shared.Extensions;
using Vero.Shared.ValueObjects;

namespace Vero.Shared.Pdf
{
    public static class PdfExtensions
    {
        public static IContainer TableDefaultCell(this TableDescriptor table) => table.Cell()
            .DefaultBorder()
            .PaddingHorizontal(2);

        public static IContainer TableDefaultTopCell(this TableDescriptor table) => table.Cell()
            .BorderTop(0.5f)
            .BorderLeft(0.5f)
            .BorderRight(0.5f)
            .BorderColor(VeroColors.DefaultBorder)
            .PaddingHorizontal(2);

        public static IContainer TableDefaultMiddleCell(this TableDescriptor table) => table.Cell()
            .BorderLeft(0.5f)
            .BorderRight(0.5f)
            .BorderColor(VeroColors.DefaultBorder)
            .PaddingHorizontal(2);

        public static IContainer TableDefaultBottomCell(this TableDescriptor table) => table.Cell()
            .BorderBottom(0.5f)
            .BorderLeft(0.5f)
            .BorderRight(0.5f)
            .BorderColor(VeroColors.DefaultBorder)
            .PaddingHorizontal(2);

        public static void DefaultText(
            this IContainer container,
            string value,
            TextStyle? textStyle = null,
            HorizontalAlignment alignment = HorizontalAlignment.Left
        ) => container.Text(
            text =>
            {
                text.DefaultTextStyle(textStyle ?? PdfTextStylesExtensions.DefaultStyle());
                text.Alignment(alignment);
                text.Span(value);
            }
        );

        public static IContainer DefaultBorder(this IContainer container, float border = 0.5f) => container.Border(border)
            .BorderColor(VeroColors.DefaultBorder);

        public static IContainer SummaryBorder(this IContainer container, float border = 0.5f) => container
           .BorderRight(border)
           .BorderColor(VeroColors.DefaultBorder);

        public static IContainer DefaultBorderBottom(this IContainer container, float border = 0.5f) => container.BorderBottom(border)
            .BorderColor(VeroColors.DefaultBorder);

        public static IContainer DefaultBorderTop(this IContainer container, float border = 0.5f) => container.BorderTop(border)
            .BorderColor(VeroColors.DefaultBorder);

        public static IContainer DefaultHeaderCell(this TableCellDescriptor table) => table.Cell()
            .Padding(8);

        public static IContainer HeaderCellStyle(this TableCellDescriptor table) => table.Cell()
            .BorderColor(VeroColors.DefaultBorder)
            .BorderRight(0.5f)
            .BorderBottom(0.5f)
            .AlignCenter()
            .Padding(4);
        public static IContainer HeaderCellStyleWithTopBorder(this TableCellDescriptor table) => table.Cell()
            .BorderColor(VeroColors.DefaultBorder)
            .BorderTop(0.5f)
            .BorderRight(0.5f)
            .BorderBottom(0.5f)
            .AlignCenter()
            .Padding(4);

        public static IContainer TableCellStyle(this TableDescriptor table) => table.Cell()
            .BorderColor(VeroColors.DefaultBorder)
            .BorderRight(0.5f)
            .BorderBottom(0.5f)
            .Padding(4)
            .ShowOnce();

        public static IContainer TableSpanCell(this TableDescriptor table, uint span) => table.Cell()
            .RowSpan(span)
            .BorderColor(VeroColors.DefaultBorder)
            .BorderRight(0.5f)
            .BorderBottom(0.5f)
            .Padding(4)
            .ShowOnce();

        public static IContainer DefaultCellContent(this TableDescriptor table) => table.Cell()
            .PaddingHorizontal(8)
            .PaddingBottom(4);

        public static IContainer TableInvisibleCell(this TableDescriptor table, HorizontalAlignment alignment = HorizontalAlignment.Left) => table.Cell()
            .Border(0)
            .BorderColor(Colors.White);

        public static void WaterMark(this PageDescriptor page, string text, bool showIf = false) => page.Background()
            .ShowIf(showIf)
            .ExtendHorizontal()
            .ExtendVertical()
            .AlignMiddle()
            .AlignCenter()
            .PaddingLeft(68)
            .Decoration(
                d =>
                {
                    d.Before(
                        b => b.Rotate(-45)
                            .Border(1)
                            .BorderColor(VeroColors.WatermarkColor)
                            .PaddingHorizontal(8)
                            .Text(text)
                            .Style(PdfTypography.WatermarkText)
                    );
                }
            );

        public static TextDescriptor Alignment(this TextDescriptor textDescriptor, HorizontalAlignment horizontalAlignment)
        {
            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    textDescriptor.AlignLeft();
                    break;

                case HorizontalAlignment.Center:
                    textDescriptor.AlignCenter();
                    break;

                case HorizontalAlignment.Right:
                    textDescriptor.AlignRight();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(horizontalAlignment), horizontalAlignment, null);
            }

            return textDescriptor;
        }

        public static void DefaultText(this IContainer container, Func<string>? value, HorizontalAlignment alignment, TextStyle style) => container.Text(
            text =>
            {
                text.DefaultTextStyle(style);
                text.Alignment(alignment);
                text.Span(value?.Invoke());
            }
        );

        public static string ToCultureDate(this DateTimeOffset datetime, CultureInfo culture, TimeZoneInfo timezone) => datetime
            .ToOffset(timezone.BaseUtcOffset)
            .ToString($"{culture.DateTimeFormat.ShortDatePattern}");

        public static string ToCultureDateTime(this DateTimeOffset datetime, CultureInfo culture, TimeZoneInfo timezone) => datetime
            .ToOffset(timezone.BaseUtcOffset)
            .ToString($"{culture.DateTimeFormat.ShortDatePattern} {culture.DateTimeFormat.ShortTimePattern}");

        public static string ConvertDateTimeToCulture(this DateTimeOffset dateTime, string culture = "pl-PL") =>
            dateTime.LocalDateTime.ToString("dd.MM.yyyy HH:mm", new CultureInfo(culture));

        public static string ConvertDateToCulture(this DateTimeOffset dateTime, string culture = "pl-PL") =>
            dateTime.LocalDateTime.ToString("dd.MM.yyyy", new CultureInfo(culture));
        
        public static string ConvertDateToCulture(this DateTimeOffset? dateTime, string culture = "pl-PL") =>
            dateTime.HasValue
                ? dateTime.Value.LocalDateTime.ToString("dd.MM.yyyy", new CultureInfo(culture))
                : string.Empty;

        public static string ConvertToStringMoney(this decimal value, Currency currency, string culture = "pl-PL") =>
            $"{value.Round(2).ToString("F2", new CultureInfo(culture))} {currency.Value}";

        public static string ConvertToStringProfability(this float value, string culture = "pl-PL") =>
            $"{float.Round(value, 2).ToString("F2", new CultureInfo(culture))}%";

        public static string ConvertToStringProfability(this double value, string culture = "pl-PL") =>
            $"{double.Round(value, 2).ToString("F2", new CultureInfo(culture))}%";

        public static string ConvertToStringMoney(this Money value, string culture = "pl-PL") =>
            $"{value.Value.Round(2).ToString("F2", new CultureInfo(culture))} {value.Currency.Value}";
    }
}