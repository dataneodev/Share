using Vero.Shared.Exceptions;

namespace Vero.Shared.MartenExtensions
{
    public sealed class ItemToPatchNotFoundException : AppException
    {
        public ItemToPatchNotFoundException() : base("Element do aktualizacji nie został znaleziony w projekcji")
        {
        }
    }
}