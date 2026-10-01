using System.Reflection;
using System.Resources;
using QuestPDF.Drawing;
using Vero.Shared.Exceptions;

namespace Vero.Shared.Pdf
{
    public sealed class PdfFontsExtension
    {
        public static void RegisterFont(string fontName)
        {
            try
            {
                var resource = Assembly.GetExecutingAssembly()
                    .GetManifestResourceNames()
                    .First(p => p.Contains(fontName, StringComparison.InvariantCultureIgnoreCase));

                var fileStream = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream(resource);

                if (fileStream is null)
                    throw new MissingManifestResourceException();

                FontManager.RegisterFontWithCustomName(fontName, fileStream);
            }
            catch (Exception)
            {
                throw new PdfFontNotFoundExceptions();
            }
        }

        private sealed class PdfFontNotFoundExceptions() : AppException("Nie znaleziono czcionki dla dokumentu pdf!");
    }
}