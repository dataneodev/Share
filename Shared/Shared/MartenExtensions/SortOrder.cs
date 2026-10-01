using Vero.Shared.DDD;

namespace Vero.Shared.MartenExtensions
{
    public sealed class SortOrder : Enumeration<SortOrder>
    {
        public static SortOrder Asc = new SortOrder(1, "asc");
        public static SortOrder Desc = new SortOrder(2, "desc");

        public SortOrder(int id, string value) : base(id, value)
        {
        }
    }
}