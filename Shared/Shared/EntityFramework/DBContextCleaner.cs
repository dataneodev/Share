using Microsoft.EntityFrameworkCore;

namespace Vero.Shared.EntityFramework
{
    public interface IDBContextCleaner
    {
        void CleanDBContext(CancellationToken cancellationToken);
    }

    internal sealed class DBContextCleaner<T> : IDBContextCleaner
        where T : DbContext
    {
        private readonly T _context;

        public DBContextCleaner(T context)
        {
            _context = context;
        }

        public void CleanDBContext(CancellationToken cancellationToken) => _context.ChangeTracker.Clear();
    }
}