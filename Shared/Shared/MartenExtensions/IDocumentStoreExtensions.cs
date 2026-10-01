using System.Linq.Expressions;
using Marten;
using Marten.Pagination;
using Marten.Services;

namespace Vero.Shared.MartenExtensions
{
    public static class DocumentStoreExtensions
    {
        public static async Task<IReadOnlyList<TOutput>> FindAsync<TInput, TOutput>(
            this IDocumentStore store,
            Func<IQueryable<TInput>, IQueryable<TOutput>> queryFunc,
            CancellationToken token = default
        )
            where TInput : notnull
            where TOutput : notnull
        {
            await using var session = store.LightweightSession();
            var query = session.Query<TInput>();

            var newQuery = queryFunc?.Invoke(query) ?? query as IQueryable<TOutput>;
            return await newQuery!.ToListAsync(token)
                .ConfigureAwait(false);
        }

        public static Task<IReadOnlyList<TInput>> FindAsync<TInput>(
            this IDocumentStore store,
            Func<IQueryable<TInput>, IQueryable<TInput>> queryFunc,
            CancellationToken token = default
        )
            where TInput : notnull
        {
            return store.FindAsync<TInput, TInput>(queryFunc, token);
        }

        public static async Task<IPagedList<TOutput>> FindPagedAsync<TInput, TOutput>(
            this IDocumentStore store,
            Func<IQueryable<TInput>, IQueryable<TOutput>> queryFunc,
            int page = 1,
            int pageSize = 100,
            CancellationToken token = default
        )
            where TInput : notnull
            where TOutput : notnull
        {
            if (page <= 0)
                page = 1;

            if (pageSize <= 0)
                pageSize = 100;

            await using var session = store.LightweightSession();
            var query = session.Query<TInput>();

            var newQuery = queryFunc?.Invoke(query) ?? query as IQueryable<TOutput>;

            newQuery = newQuery!.ThenBySql("id");

            return await newQuery!.ToPagedListAsync(page, pageSize, token)
                .ConfigureAwait(false);
        }

        public static Task<IPagedList<TInput>> FindPagedAsync<TInput>(
            this IDocumentStore store,
            Func<IQueryable<TInput>, IQueryable<TInput>> queryFunc,
            int page = 1,
            int pageSize = 100,
            CancellationToken token = default
        )
            where TInput : notnull
        {
            return store.FindPagedAsync<TInput, TInput>(queryFunc, page, pageSize, token);
        }

        public static async Task<TInput?> FirstOrDefaultAsync<TInput>(
            this IDocumentStore store,
            Expression<Func<TInput, bool>> predicate,
            CancellationToken token = default
        )
            where TInput : notnull
        {
            await using var session = store.LightweightSession();
            var query = session.Query<TInput>();
            return await query.FirstOrDefaultAsync(predicate)
                .ConfigureAwait(false);
        }

        public static async Task PatchAsync<T>(this IDocumentStore store, Expression<Func<T, bool>> find, Action<T> update)
            where T : notnull
        {
            await using var session = store.LightweightSession();
            var item = await session.Query<T>()
                .FirstOrDefaultAsync(find);

            if (item is null)
                throw new ItemToPatchNotFoundException();

            update(item);
            session.Update(item);

            await session.SaveChangesAsync();
        }

        public static async Task PatchAsync<T>(this IDocumentStore store, Expression<Func<T, bool>> find, Func<T, Task> update)
            where T : notnull
        {
            await using var session = store.LightweightSession();
            var item = await session.Query<T>()
                .FirstOrDefaultAsync(find);

            if (item is null)
                throw new ItemToPatchNotFoundException();

            await update(item);
            session.Update(item);

            await session.SaveChangesAsync();
        }

        public static async Task DeleteAsync<T>(this IDocumentStore store, Expression<Func<T, bool>> find, bool throwExceptionOnNotFound = true)
            where T : notnull
        {
            await using var session = store.LightweightSession();
            var item = await session.Query<T>()
                .FirstOrDefaultAsync(find);

            if (item is null && !throwExceptionOnNotFound)
                return;

            if (item is null)
                throw new ItemToDeleteNotFoundException();

            session.Delete(item);
            await session.SaveChangesAsync();
        }

        public static async Task BatchDeleteAsync<T>(this IDocumentStore store, Expression<Func<T, bool>> find, bool throwExceptionOnNotFound = true)
            where T : notnull
        {
            await using var session = store.LightweightSession();
            var items = await session.Query<T>()
                .Where(find)
                .ToListAsync();

            if (!(items?.Any() ?? false) && !throwExceptionOnNotFound)
                return;

            if (items is null)
                throw new ItemToDeleteNotFoundException();

            if (!items.Any())
                throw new ItemToDeleteNotFoundException();

            foreach (var item in items)
            {
                session.Delete(item);
            }

            await session.SaveChangesAsync();
        }

        public static async Task SaveAsync<T>(this IDocumentStore store, T obj)
            where T : notnull
        {
            var sessionOptions = new SessionOptions { ConcurrencyChecks = ConcurrencyChecks.Disabled, Tracking = DocumentTracking.None };

            await using var session = await store.LightweightSerializableSessionAsync(sessionOptions);
            session.Store(obj);
            await session.SaveChangesAsync();
        }
    }
}