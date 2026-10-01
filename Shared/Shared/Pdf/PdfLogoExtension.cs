using System.Reflection;
using Vero.Shared.Exceptions;

namespace Vero.Shared.Pdf
{
    public sealed class PdfLogoExtension
    {
        public static Stream? GetVeroLogo()
        {
            try
            {
                var d = Assembly.GetExecutingAssembly()
                    ?.GetManifestResourceNames()
                    .First(p => p.Contains("verocargo_logo", StringComparison.InvariantCultureIgnoreCase));

                if (d == null)
                    throw new VeroLogoNotFountException();

                return Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream(d);
            }
            catch (Exception)
            {
                throw new VeroLogoNotFountException();
            }
        }

        private sealed class VeroLogoNotFountException : AppException
        {
            public VeroLogoNotFountException() : base("Nie znaleziono logo VEROCARGO!")
            {
            }
        }
    }
}