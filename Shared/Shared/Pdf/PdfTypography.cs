using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Vero.Shared.Pdf
{
    public static class PdfTypography
    {
        public static TextStyle Title => TextStyle.Default.FontColor(VeroColors.TextColor).FontSize(16).FontFamily("Roboto");
        public static TextStyle InvoiceSectionTitle => TextStyle.Default.FontColor(VeroColors.TextColor).FontSize(12).FontFamily("Roboto");
        public static TextStyle SectionTitle => TextStyle.Default.FontColor(VeroColors.TextColor).FontSize(10).FontFamily("Roboto");
        public static TextStyle BoldSubtitle => TextStyle.Default.FontColor(VeroColors.TextColor).FontSize(10).SemiBold().FontFamily("Roboto");
        public static TextStyle DefaultBoldText => TextStyle.Default.FontColor(VeroColors.TextColor).FontSize(8).SemiBold().FontFamily("Roboto");
        public static TextStyle DefaultText => TextStyle.Default.FontColor(VeroColors.TextColor).FontSize(8).FontFamily("Roboto");
        public static TextStyle WatermarkText => TextStyle.Default.FontColor(VeroColors.WatermarkColor).FontSize(48).FontFamily("Roboto");
    }
}