using Vero.Shared.Exceptions;

namespace Vero.Shared.MartenExtensions
{
    public sealed class ItemToDeleteNotFoundException : AppException
    {
        public ItemToDeleteNotFoundException() : base("Element do usunięcia nie został znaleziony w projekcji")
        {
        }
    }
}