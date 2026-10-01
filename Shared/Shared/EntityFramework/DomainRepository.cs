using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Vero.Shared.DDD;
using Vero.Shared.Exceptions;

namespace Vero.Shared.EntityFramework
{
    public interface IDomainRepository<TEntity, TEntityId>
        where TEntity : Entity<TEntityId>
        where TEntityId : EntityId
    {
        Task<TEntity?> GetAsync(TEntityId id);

        Task<TEntity> GetOrThrowAsync<TException>(TEntityId id)
            where TException : AppException;

        Task AddAsync(TEntity entity);

        Task<TEntity?> ReadAsync(TEntityId id, CancellationToken ct = default);

        Task<TEntity> ReadOrThrowAsync<TException>(TEntityId id, CancellationToken ct = default)
            where TException : AppException;

        Task<IReadOnlyList<TEntity>> ReadAsync(IEnumerable<TEntityId> ids, CancellationToken ct = default);

        Task<IReadOnlyList<TEntity>> ReadOrThrowAsync<TException>(IEnumerable<TEntityId> ids, CancellationToken ct = default)
            where TException : AppException;

        Task<TMap?> ReadAsync<TMap>(TEntityId id, Expression<Func<TEntity, TMap>> selector, CancellationToken ct = default);

        Task<TMap> ReadOrThrowAsync<TMap, TException>(TEntityId id, Expression<Func<TEntity, TMap>> selector, CancellationToken ct = default)
            where TException : AppException;

        Task<IReadOnlyList<TMap>> ReadAsync<TMap>(IEnumerable<TEntityId> ids, Expression<Func<TEntity, TMap>> selector, CancellationToken ct = default);

        Task<IReadOnlyList<TMap>> ReadOrThrowAsync<TMap, TException>(
            IEnumerable<TEntityId> ids,
            Expression<Func<TEntity, TMap>> selector,
            Func<TMap, TEntityId> idSelector,
            CancellationToken ct = default
        )
            where TException : AppException;

        Task<T?> FindFirstAsync<T>(Func<IQueryable<TEntity>, IQueryable<T>> query, CancellationToken ct = default)
            where T : class;

        Task<IReadOnlyList<T>> FindAsync<T>(Func<IQueryable<TEntity>, IQueryable<T>> query, CancellationToken ct = default)
            where T : class;

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> query, CancellationToken ct = default);

        Task<int> CountAsync(Expression<Func<TEntity, bool>> query, CancellationToken ct = default);

        IQueryable<TTEntity> GetReadOnlyEntity<TTEntity>()
            where TTEntity : Entity;
    }

    public abstract class DomainRepository<TEntity, TEntityId>(DbContext dbContext) : IDomainRepository<TEntity, TEntityId>
        where TEntity : Entity<TEntityId>
        where TEntityId : EntityId
    {
        protected readonly DbContext _dbContext = dbContext;

        public virtual async Task<TEntity?> GetAsync(TEntityId id)
        {
            return await _dbContext.Set<TEntity>()
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbContext.AddAsync(entity);
        }

        public async Task<TEntity> GetOrThrowAsync<TException>(TEntityId id)
            where TException : AppException
        {
            return await GetAsync(id) ?? throw (Activator.CreateInstance(typeof(TException), id) as AppException)!;
        }

        public async Task<TEntity?> ReadAsync(TEntityId id, CancellationToken ct = default)
        {
            return await _dbContext.Set<TEntity>()
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id, ct);
        }

        public async Task<TEntity> ReadOrThrowAsync<TException>(TEntityId id, CancellationToken ct = default)
            where TException : AppException
        {
            return await ReadAsync(id, ct) ?? throw (Activator.CreateInstance(typeof(TException), id) as AppException)!;
        }

        public async Task<IReadOnlyList<TEntity>> ReadAsync(IEnumerable<TEntityId> ids, CancellationToken ct = default)
        {
            var idsArray = ids.ToHashSet();

            return await _dbContext.Set<TEntity>()
                .AsNoTracking()
                .Where(w => idsArray.Contains(w.Id))
                .ToArrayAsync(ct);
        }

        public async Task<IReadOnlyList<TEntity>> ReadOrThrowAsync<TException>(IEnumerable<TEntityId> ids, CancellationToken ct = default)
            where TException : AppException
        {
            var idsArray = ids.ToHashSet();
            var models = await ReadAsync(idsArray, ct);

            if (idsArray.Except(models.Select(s => s.Id))
                .Any())
            {
                throw (Activator.CreateInstance(
                    typeof(TException),
                    idsArray.Except(models.Select(s => s.Id))
                        .First()
                ) as AppException)!;
            }

            return models;
        }

        public async Task<TMap?> ReadAsync<TMap>(TEntityId id, Expression<Func<TEntity, TMap>> selector, CancellationToken ct = default)
        {
            return await _dbContext.Set<TEntity>()
                .AsNoTracking()
                .Where(w => w.Id == id)
                .Take(1)
                .Select(selector)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<TMap> ReadOrThrowAsync<TMap, TException>(TEntityId id, Expression<Func<TEntity, TMap>> selector, CancellationToken ct = default)
            where TException : AppException
        {
            return await ReadAsync(id, selector, ct) ?? throw (Activator.CreateInstance(typeof(TException), id) as AppException)!;
        }

        public async Task<IReadOnlyList<TMap>> ReadAsync<TMap>(
            IEnumerable<TEntityId> ids,
            Expression<Func<TEntity, TMap>> selector,
            CancellationToken ct = default
        )
        {
            var idsArray = ids.ToHashSet();

            return await _dbContext.Set<TEntity>()
                .AsNoTracking()
                .Where(w => idsArray.Contains(w.Id))
                .Select(selector)
                .ToArrayAsync(ct);
        }

        public async Task<IReadOnlyList<TMap>> ReadOrThrowAsync<TMap, TException>(
            IEnumerable<TEntityId> ids,
            Expression<Func<TEntity, TMap>> selector,
            Func<TMap, TEntityId> idSelector,
            CancellationToken ct = default
        )
            where TException : AppException
        {
            var idsArray = ids.ToHashSet();
            var models = await ReadAsync(idsArray, selector, ct);

            if (idsArray.Except(models.Select(idSelector))
                .Any())
            {
                throw (Activator.CreateInstance(
                    typeof(TException),
                    idsArray.Except(models.Select(idSelector))
                        .First()
                ) as AppException)!;
            }

            return models;
        }

        public async Task<T?> FindFirstAsync<T>(Func<IQueryable<TEntity>, IQueryable<T>> query, CancellationToken ct = default)
            where T : class
        {
            var q = _dbContext.Set<TEntity>()
                .AsNoTracking();

            return await query(q)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<IReadOnlyList<T>> FindAsync<T>(Func<IQueryable<TEntity>, IQueryable<T>> query, CancellationToken ct = default)
            where T : class
        {
            var q = _dbContext.Set<TEntity>()
                .AsNoTracking();

            return await query(q)
                .ToArrayAsync(ct);
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> query, CancellationToken ct = default)
        {
            return await _dbContext.Set<TEntity>()
                .AsNoTracking()
                .AnyAsync(query, ct);
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> query, CancellationToken ct = default)
        {
            return await _dbContext.Set<TEntity>()
                .AsNoTracking()
                .CountAsync(query, ct);
        }

        public IQueryable<TTEntity> GetReadOnlyEntity<TTEntity>()
            where TTEntity : Entity
        {
            return _dbContext.Set<TTEntity>()
                .AsNoTracking();
        }
    }
}