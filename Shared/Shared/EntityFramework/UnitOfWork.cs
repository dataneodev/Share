using Microsoft.EntityFrameworkCore;
using Vero.Shared.Abstractions;

namespace Vero.Shared.EntityFramework
{
    internal sealed class UnitOfWork<T> : IUnitOfWork
        where T : DbContext
    {
        private readonly T _context;

        public UnitOfWork(T context)
        {
            _context = context;
        }

        public async Task Save(CancellationToken cancellationToken)
        {
            // _context.ChangeTracker.ThrowOnMultipleOwnsOneWithTheSameShadowIdObjects();
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}