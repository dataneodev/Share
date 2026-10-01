using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Vero.Shared.Pdf
{
    public static class PdfTextStylesExtensions
    {
        public static TextStyle DefaultStyle(uint fontSize = 8) => TextStyle.Default.FontSize(fontSize)
            .FontColor(VeroColors.DefaultFont).FontFamily("Roboto");

        public static TextStyle Bold(uint fontSize = 8) => DefaultStyle(fontSize)
            .Bold().FontFamily("Roboto");
    }
}